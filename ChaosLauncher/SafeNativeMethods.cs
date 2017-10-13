// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.Runtime.InteropServices;

namespace ChaosLauncher
{
    internal static class SafeNativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SetWindowText(IntPtr hWnd, string text);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool CreateProcess(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, ProcessCreationFlags creationFlags, IntPtr environment, string currentDirectory, ref StartInfo startupInfo, out ProcInfo processInfo);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr OpenProcess(ProcessAccess access, bool inheritHandle, uint processId);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr baseAddress, IntPtr buffer, IntPtr count, out int bytesRead);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr baseAddress, IntPtr buffer, IntPtr count, out int bytesWritten);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern uint WaitForSingleObject(IntPtr hObject, uint timeout);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, UIntPtr dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
