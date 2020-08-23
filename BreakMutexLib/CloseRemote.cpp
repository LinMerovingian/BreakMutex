#include "pch.h"
#include "CloseRemote.h"

BOOL CALLBACK EnumWndProc(HWND hWnd, LPARAM lParam)
{
	std::wstring buff;
	buff.resize(titleLen);
	::GetWindowText(hWnd, &buff[0], titleLen);
	buff.resize(titleLen - 1);
	if (buff.find(((cell*)lParam)->WindowName, 0) != std::wstring::npos)
	{
		((cell*)lParam)->hWnd = hWnd;
		((cell*)lParam)->WindowName.clear();
		((cell*)lParam)->WindowName = buff.c_str();
	}
	return true;
}

bool CloseRemote(ULONG dwProcessId, PCWSTR Name)
{
	auto rtn = false;
	// create any file
	HANDLE hFile = OpenMutex(MAXIMUM_ALLOWED, FALSE, Name);

	if (hFile != INVALID_HANDLE_VALUE)
	{
		if (HANDLE hProcess = OpenProcess(PROCESS_DUP_HANDLE, FALSE, dwProcessId))
		{

			NTSTATUS status;
			ULONG cb = 0x80000;

			union {
				PSYSTEM_HANDLE_INFORMATION_EX pshi;
				PVOID pv;
			};

			do
			{
				status = STATUS_INSUFFICIENT_RESOURCES;

				if (pv = ::LocalAlloc(0, cb))
				{
					if (0 <= (status = NtQuerySystemInformation(SystemExtendedHandleInformation, pv, cb, &cb)))
					{
						if (ULONG_PTR NumberOfHandles = pshi->NumberOfHandles)
						{
							ULONG_PTR UniqueProcessId = ::GetCurrentProcessId();
							PSYSTEM_HANDLE_TABLE_ENTRY_INFO_EX Handles = pshi->Handles;
							do
							{
								// ここで対象プロセスと一致してるか見てる
								// search for created file
								if (Handles->UniqueProcessId == UniqueProcessId && Handles->HandleValue == (ULONG_PTR)hFile)
								{
									// we got it !
									PVOID Object = Handles->Object;

									NumberOfHandles = pshi->NumberOfHandles, Handles = pshi->Handles;
									do
									{
										if (Object == Handles->Object && Handles->UniqueProcessId == dwProcessId)
										{
											// ここでmutex破壊してる
											// TODO:戻り値確認、ここで破棄確認出来る？
											rtn = ::DuplicateHandle(hProcess, (HANDLE)Handles->HandleValue, 0, 0, 0, 0, DUPLICATE_CLOSE_SOURCE);
										}

									} while (Handles++, --NumberOfHandles);

									break;
								}
							} while (Handles++, --NumberOfHandles);
						}
					}
					LocalFree(pv);
				}

			} while (status == STATUS_INFO_LENGTH_MISMATCH);

			::CloseHandle(hProcess);
		}

		::CloseHandle(hFile);
		return rtn;
	}
}