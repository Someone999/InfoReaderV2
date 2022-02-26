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
        private static readonly Dictionary<Type, (Task, Form)> MessageLoopingWindows = new();
        public bool AutoCatch => true;
        public string MainCommand => "config";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
        

        public string GetHelp()
        {
            return LocalizationInfo.Current.Translations["LANG_HELP_CONFIGCOMMAND"];
        }


        private void OpenConfig(InfoReaderPlugin instance, CommandParser parser)
        {
            var args = parser.Arguments;
            if (instance.Configurables.ContainsKey(args[0]))
            {
                var config = instance.Configurables[args[0]];
                if (config is IGuiConfigurable guiConfigurable)
                {
                    Type configType = config.GetType();
                    if (MessageLoopingWindows.ContainsKey(configType))
                    {
                        var t = MessageLoopingWindows[configType];
                        if (t.Item1.IsCompleted)
                        {
                            MessageLoopingWindows.Remove(configType);
                            var form = guiConfigurable.CreateConfigWindow();
                            var startedTask = WindowTools.StartMessageLoop(form);
                            MessageLoopingWindows.Add(configType, startedTask);
                        }
                    }
                    else
                    {
                        var form = guiConfigurable.CreateConfigWindow();
                        var startedTask = WindowTools.StartMessageLoop(form);
                        MessageLoopingWindows.Add(configType, startedTask);
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
                        foreach (var element in instance.ConfigElements)
                        {
                            if (element.Value is IConfigSerializer and IConfigWriter writer)
                            {
                                writer.WriteToFile("D:\\a\\s\\config.txt");
                            }
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
