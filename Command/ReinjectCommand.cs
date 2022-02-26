using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Command.Parser;
using InfoReader.Tools;
using InfoReader.Tools.I8n;
using InfoReader.Tools.Win32;

namespace InfoReader.Command
{
    public class ReinjectCommand : ICommandProcessor
    {
        public bool AutoCatch => true;
        public string MainCommand => "reinject";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
        
        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            var processes = ProcessTools.FindProcess("osu!");
            if (processes.Length == 0)
            {
                if (string.IsNullOrEmpty(instance.Configuration.GameDirectory))
                {
                    Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_NOPROCESS"]);
                    return true;
                }
                OpenCommand.Open(instance.Configuration.GameDirectory);
            }

            if (ProcessTools.ContainsModule(processes[0], "Overlay.dll"))
            {
                Logger.LogError(LocalizationInfo.Current.Translations["LANG_INFO_HASINJECTED"]);
                return true;
            }
            IInjector injector = new Injector();
            var osu = processes[0];
            var path = $"{Environment.CurrentDirectory}\\..\\overlay.dll";
            if (!injector.Inject(osu.Id, path))
            {
                injector = new CppInjector();
                Logger.LogNotification(LocalizationInfo.Current.Translations["LANG_INFO_INJECTED"]);
                return injector.Inject(osu.Id, path);
            }
            Logger.LogNotification(LocalizationInfo.Current.Translations["LANG_INFO_INJECTED"]);
            return true;
        }
    }
}
