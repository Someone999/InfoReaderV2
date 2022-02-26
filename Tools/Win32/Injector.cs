using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InfoReader.Tools.Win32;

public class Injector:IInjector
{
    private IntPtr GetLoadLibrary()
    {
        IntPtr hKernel32 = NativeApi.GetModuleHandle("kernel32");
        return NativeApi.GetProcAddress(hKernel32, "LoadLibraryW");
    }

    private (IntPtr,int) WriteDllPath(IntPtr hProcess, string modulePath)
    {
        byte[] bts = Encoding.UTF8.GetBytes(modulePath);
        IntPtr remote = NativeApi.VirtualAllocEx(hProcess, IntPtr.Zero, (uint) bts.Length, NativeApi.AllocationType.Commit,
            NativeApi.MemoryProtection.ReadWrite);
        if (remote == IntPtr.Zero)
        {
            Logger.LogError($"[VirtualAllocEx] Failed. Error code: {Marshal.GetLastWin32Error()}");
            return (IntPtr.Zero,0);
        }
        IntPtr path = Marshal.AllocHGlobal(modulePath.Length + 10);
            
        Marshal.Copy(bts, 0, path, bts.Length);
            
        int realLen = 0;
        bool suc = NativeApi.WriteProcessMemory(hProcess, remote, path, bts.Length, ref realLen);
        if (suc && realLen == bts.Length)
            return (path,bts.Length);
        Logger.LogError($"[WriteProcessMemory] Failed. Error code: {Marshal.GetLastWin32Error()}");
        return (IntPtr.Zero,0);

    }
    public bool Inject(int pid, string modulePath)
    {
        IntPtr process = NativeApi.OpenProcess(NativeApi.ProcessAccessFlags.All, false, pid);
        if (process == IntPtr.Zero)
        {
            Logger.LogError($"[OpenProcess] Failed. Error code: {Marshal.GetLastWin32Error()}");
            return false;
        }

        var remoteBuffer = WriteDllPath(process, modulePath);
        if (remoteBuffer.Item1 == IntPtr.Zero)
        {
            return false;
        }

        IntPtr hThread = NativeApi.CreateRemoteThread(process, IntPtr.Zero, 0, GetLoadLibrary(), remoteBuffer.Item1,
            0,
            IntPtr.Zero);
        if (hThread == IntPtr.Zero) 
            return false;
        NativeApi.WaitForSingleObject(hThread, 0xFFFFFFFF);
        NativeApi.VirtualFreeEx(process, remoteBuffer.Item1, remoteBuffer.Item2,
            NativeApi.AllocationType.Decommit);
        NativeApi.CloseHandle(hThread);
        return true;

    }
}