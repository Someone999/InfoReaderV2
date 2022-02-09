using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InfoReader.Tools.Win32
{
    public class CppInjector: IInjector
    {
        public bool Inject(int pid, string? modulePath)
        {
            Process osuprocess = Process.GetProcessById(pid);
            var cinjector = System.Diagnostics.Process.Start(
                new ProcessStartInfo
                {
                    FileName = "injector",
                    Arguments = $"\"{modulePath}\" {osuprocess.Id}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            Thread.Sleep(3000);
            return ProcessTools.ContainsModule(osuprocess, "Overlay.dll");
        }
    }
}
