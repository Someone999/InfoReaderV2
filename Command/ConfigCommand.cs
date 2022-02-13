using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfoReader.Command.Parser;
using InfoReader.Configuration;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Configuration.Gui;
using InfoReader.Configuration.Serializer;
using InfoReader.Mmf;
using InfoReader.Tools;
using InfoReader.Tools.I8n;

namespace InfoReader.Command
{
    public class ConfigCommand:ICommandProcessor
    {
        private static Dictionary<Type, (Task, Form)> _messageLoopingWindows = new Dictionary<Type, (Task, Form)>();
        public bool AutoCatch => true;
        public string MainCommand => "config";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
        

        public string GetHelp()
        {
            return LocalizationInfo.Current.Translations["LANG_HELP_CONFIGCOMMAND"];
        }

       
        void OpenConfig(InfoReaderPlugin instance, CommandParser parser)
        {
            var args = parser.Arguments;
            if (instance.Configurables.ContainsKey(args[0]))
            {
                var config = instance.Configurables[args[0]];
                if (config is IGuiConfigurable guiConfigurable)
                {
                    Type configType = config.GetType();
                    if (_messageLoopingWindows.ContainsKey(configType))
                    {
                        var t = _messageLoopingWindows[configType];
                        if (t.Item1.IsCompleted)
                        {
                            _messageLoopingWindows.Remove(configType);
                            var form = guiConfigurable.CreateConfigWindow();
                            var startedTask = WindowTools.StartMessageLoop(form);
                            _messageLoopingWindows.Add(configType, startedTask);
                        }
                    }
                    else
                    {
                        var form = guiConfigurable.CreateConfigWindow();
                        var startedTask = WindowTools.StartMessageLoop(form);
                        _messageLoopingWindows.Add(configType, startedTask);
                    }
                }
                else
                {
                    OpenCommand.Open(DefaultFilePath.CurrentConfigFile);
                    Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_NOGUI"]);
                }
            }
        }
        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            var args = parser.Arguments;
            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "refresh":
                        ConfigTools.RefreshConfig(instance);
                        return true;
                    case "restore":
                        if (instance.ConfigElement is IConfigSerializer and IConfigWriter writer)
                        {
                            writer.WriteToFile(DefaultFilePath.CurrentConfigFile);
                        }
                        ConfigTools.RefreshConfig(instance);
                        return true;
                    case "open":
                        OpenCommand.Open(DefaultFilePath.CurrentConfigFile);
                        return true;
                    default:
                        OpenConfig(instance, parser);
                        return true;
                }
                
            }
            else if (args.Length > 1)
            {
                
            }
            else if (args.Length > 2)
            {
                switch (args[0])
                {
                    case "mmf":
                        var mmfName = args[1];
                        MmfBase? mmf = MmfManager.GetInstance(instance).FindMmf(mmfName);
                        if (mmf == null)
                            return true;

                        mmf.Enabled = args[2] switch
                        {
                            "disable" => false,
                            "false" => false,
                            "enable" => true,
                            "true" => true,
                            _ => false
                        };
                        break;

                }
            }
            return true;
        }
    }
}
