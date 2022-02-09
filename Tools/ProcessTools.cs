using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (module.ModuleName.ToLower() == moduleName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
