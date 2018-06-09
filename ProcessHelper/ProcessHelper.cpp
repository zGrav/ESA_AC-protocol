// This is the main DLL file.

#include "stdafx.h"

#include "ProcessHelper.h"

#include <cstdio>
#include <windows.h>
#include <tlhelp32.h>

using namespace System;
using namespace System::Collections::Generic;
namespace NativeProcessHelper
{
	public ref class Helper
	{
	public:
		//http://www.mpgh.net/forum/161-programming-tutorials/63802-c-get-process-id-name.html
		static System::Int32 ContainsProcess(WCHAR ProcName[260])
		{
			PROCESSENTRY32 entry;
			entry.dwSize = sizeof(PROCESSENTRY32);

			HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);

			if (Process32First(snapshot, &entry) == TRUE)
			{
				while (Process32Next(snapshot, &entry) == TRUE)
				{
					if (_wcsicmp(entry.szExeFile, ProcName) == 0)
					{
						CloseHandle(snapshot);
						return 1;
					}
				}
			}

			CloseHandle(snapshot);
			return 0;
		}

		static List<int>^ GetPIDList()
		{
			List<int>^ processIDs = gcnew List<int>();
			//th32ProcessID

			PROCESSENTRY32 entry;
			entry.dwSize = sizeof(PROCESSENTRY32);

			HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);

			if (Process32First(snapshot, &entry) == TRUE)
			{
				while (Process32Next(snapshot, &entry) == TRUE)
				{
					processIDs->Add((int)entry.th32ProcessID);
				}
			}

			CloseHandle(snapshot);

			return processIDs;
		}
	};
}