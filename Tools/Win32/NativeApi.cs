using System;
using System.Runtime.InteropServices;

namespace InfoReader.Tools.Win32
{

    internal static class NativeApi
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }
        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierFlag = 0x100,
            NoCacheModifierFlag = 0x200,
            WriteCombineModifierFlag = 0x400
        }
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(
            ProcessAccessFlags processAccess,
            bool bInheritHandle,
            int processId
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr processHandle, IntPtr baseAddress, IntPtr buffer, int size, ref int realLength);
        [DllImport("kernel32.dll")]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes, uint dwStackSize, IntPtr
                lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
            int dwSize, AllocationType dwFreeType);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr GetProcAddress(IntPtr dllHandle, [MarshalAs(UnmanagedType.LPStr)] string lpFuncName);
        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleA", SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string moduleName);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);
    }
}
