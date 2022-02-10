using System;
using System.Collections.Generic;
using System.Linq;
using InfoReader.Command.Parser;
using InfoReader.Configuration;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Configuration.Serializer;
using InfoReader.Mmf;

namespace InfoReader.Command
{
    public class ConfigCommand:ICommandProcessor
    {
        public bool AutoCatch => true;
        public string MainCommand => "config";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
        

        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        

        public static void SetConfig(string configPath, object val, IConfigElement element)
        {
            var parts = configPath.Split('.');
            IConfigElement tmp = element;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                tmp = tmp[parts[i]];
            }

            tmp.SetValue(parts.Last(), val);
        }

        public void RefreshConfig(InfoReaderPlugin instance)
        {
            MmfManager.CreateNewInstance(instance);
            Dictionary<Type, object[]> converterTypesArgs = new Dictionary<Type, object[]>
            {
                {typeof(MmfsConverter), new object[] {this}},
                {typeof(BeatmapReadMethodsConverter), new object[]{this}}
            };
            IConfigElement element = new TomlConfigElement("InfoReaderConfig.toml");
            InfoReaderPlugin.InitConfig(instance.Configuration, element, converterTypesArgs);
            InfoReaderPlugin.InitConfig(instance.MmfConfiguration, element, converterTypesArgs);
            
            var mmfMgr = MmfManager.GetInstance(instance);
            mmfMgr.StopUpdate();
            mmfMgr.StartUpdate(100);
        }
        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            var args = parser.Arguments;
            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "refresh":
                        RefreshConfig(instance);
                        break;
                    case "restore":
                        if (instance.ConfigElement is IConfigSerializer and IConfigWriter writer)
                        {
                            writer.WriteToFile("InfoReaderConfig.toml");
                        }
                        RefreshConfig(instance);
                        break;
                    case "open":
                        OpenCommand.Open("InfoReaderConfig.toml");
                        break;
                }
            }
            else if (args.Length > 2)
            {
                if (args[0] == "mmf")
                {
                    var mmfName = args[1];
                    MmfBase? b = MmfManager.GetInstance(instance).FindMmf(mmfName);
                    if (b == null)
                        return true;
                    b.Enabled = bool.Parse(args[2]);
                }
            }
            return true;
        }
    }
}
