#include "pch.h"
#include "CloseRemote.h"

// 参考：https://support.microsoft.com/ja-jp/help/131065/how-to-obtain-a-handle-to-any-process-with-sedebugprivilege
bool SetSeDebugPrivilege()
{
	HANDLE hToken;

	if (!OpenThreadToken(GetCurrentThread(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, FALSE, &hToken))
	{
		if (GetLastError() == ERROR_NO_TOKEN)
		{
			if (!ImpersonateSelf(SecurityImpersonation))
				return false;

			if (!OpenThreadToken(GetCurrentThread(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, FALSE, &hToken))
				return false;
		}
		else
			return false;
	}

	// enable SeDebugPrivilege
	if (!SetPrivilege(hToken, SE_DEBUG_NAME, TRUE))
	{
		// close token handle
		CloseHandle(hToken);

		// indicate failure
		return false;
	}

	// close handles
	CloseHandle(hToken);

	return true;
}


bool SetPrivilege(
	HANDLE hToken,          // token handle
	LPCTSTR Privilege,      // Privilege to enable/disable
	bool bEnablePrivilege   // TRUE to enable.  FALSE to disable
)
{
	TOKEN_PRIVILEGES tp;
	LUID luid;
	TOKEN_PRIVILEGES tpPrevious;
	DWORD cbPrevious = sizeof(TOKEN_PRIVILEGES);

	if (!LookupPrivilegeValue(NULL, Privilege, &luid))
		return false;

	// 
	// first pass.  get current privilege setting
	// 
	tp.PrivilegeCount = 1;
	tp.Privileges[0].Luid = luid;
	tp.Privileges[0].Attributes = 0;

	AdjustTokenPrivileges(
		hToken,
		FALSE,
		&tp,
		sizeof(TOKEN_PRIVILEGES),
		&tpPrevious,
		&cbPrevious
	);

	if (GetLastError() != ERROR_SUCCESS)
		return false;

	// 
	// second pass.  set privilege based on previous setting
	// 
	tpPrevious.PrivilegeCount = 1;
	tpPrevious.Privileges[0].Luid = luid;

	if (bEnablePrivilege)
		tpPrevious.Privileges[0].Attributes |= (SE_PRIVILEGE_ENABLED);
	else
	{
		tpPrevious.Privileges[0].Attributes ^= (SE_PRIVILEGE_ENABLED &
			tpPrevious.Privileges[0].Attributes);
	}

	AdjustTokenPrivileges(
		hToken,
		FALSE,
		&tpPrevious,
		cbPrevious,
		NULL,
		NULL
	);

	return GetLastError() == ERROR_SUCCESS;
}


bool CloseRemote(ULONG dwProcessId, PCWSTR Name)
{
	auto rtn = false;

	// create any file
	HANDLE hFile = OpenMutex(MAXIMUM_ALLOWED, FALSE, Name);

	if (hFile != NULL && hFile != INVALID_HANDLE_VALUE)
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
	}
	return rtn;
}