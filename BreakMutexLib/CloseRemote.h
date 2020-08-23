#pragma once

#ifdef MATHLIBRARY_EXPORTS
#define MATHLIBRARY_API __declspec(dllexport)
#else
#define MATHLIBRARY_API __declspec(dllimport)
#endif

typedef
__kernel_entry NTSYSCALLAPI
NTSTATUS
(NTAPI
	* NtQueryObject_t)(
		_In_opt_ HANDLE Handle,
		_In_ OBJECT_INFORMATION_CLASS ObjectInformationClass,
		_Out_writes_bytes_opt_(ObjectInformationLength) PVOID ObjectInformation,
		_In_ ULONG ObjectInformationLength,
		_Out_opt_ PULONG ReturnLength
		);

typedef
__kernel_entry NTSTATUS
(NTAPI
	* NtQuerySystemInformation_t)(
		IN SYSTEM_INFORMATION_CLASS SystemInformationClass,
		OUT PVOID SystemInformation,
		IN ULONG SystemInformationLength,
		OUT PULONG ReturnLength OPTIONAL
		);

typedef struct _SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX
{
	PVOID Object;
	ULONG_PTR UniqueProcessId;
	ULONG_PTR HandleValue;
	ULONG GrantedAccess;
	USHORT CreatorBackTraceIndex;
	USHORT ObjectTypeIndex;
	ULONG HandleAttributes;
	ULONG Reserved;
} SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX, * PSYSTEM_HANDLE_TABLE_ENTRY_INFO_EX;

typedef struct _SYSTEM_HANDLE_INFORMATION_EX
{
	ULONG_PTR  NumberOfHandles;
	ULONG_PTR  Reserved;
	SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX Handles[1];
} SYSTEM_HANDLE_INFORMATION_EX, * PSYSTEM_HANDLE_INFORMATION_EX;

static const SYSTEM_INFORMATION_CLASS SystemExtendedHandleInformation = static_cast<SYSTEM_INFORMATION_CLASS>(0x40);

const auto titleLen = 1024;

struct cell
{
	HWND hWnd;
	std::wstring WindowName;
};

BOOL CALLBACK EnumWndProc(HWND hWnd, LPARAM lParam);
extern "C" MATHLIBRARY_API bool CloseRemote(ULONG dwProcessId, PCWSTR Name);