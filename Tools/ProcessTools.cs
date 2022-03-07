using System.Diagnostics;

namespace InfoReader.Tools
{
    public static class ProcessTools
    {
        public static Process[] FindProcess(string processName)
        {
            return Process.GetProcessesByName(processName);
        }

        public static bool ContainsModule(Process process, string moduleName)
        {
            var modules = process.Modules;
            foreach (ProcessModule module in modules)
            {
                if (module.ModuleName.ToLower() == moduleName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public static Process? StartProcess(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName);
            return Process.Start(startInfo);
        }
    }
}
