using System;
using System.IO;
using System.Threading;
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
                if (string.IsNullOrEmpty(instance.Configuration.GamePath))
                {
                    Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_NOPROCESS"]);
                    return true;
                }
                OpenCommand.Open(instance.Configuration.GamePath);
                Thread.Sleep(3000);
            }

            if (ProcessTools.ContainsModule(processes[0], "Overlay.dll"))
            {
                Logger.LogError(LocalizationInfo.Current.Translations["LANG_INFO_HASINJECTED"]);
                return true;
            }
            IInjector injector = new Injector();
            var osu = processes[0];
            var related = instance.Configurables["program"]?.ConfigElement?["Program"]["OverlayDllPath"]
                .GetValue<string>() ?? throw new InvalidOperationException();
            var path = Path.GetFullPath(related);
            if (!File.Exists(path))
            {
                string? directory = Path.GetDirectoryName(path);
                if (directory == null)
                    throw new InvalidOperationException();
                string fileName = Path.GetFileName(path);
                if(fileName == null)
                    throw new InvalidOperationException();
                directory = Path.GetFullPath(Path.Combine(directory, ".."));
                path = Path.Combine(directory, fileName);
            }

            instance.Configurables["program"]?.ConfigElement?["Program"].SetValue("OverlayDllPath", path);
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
