#define TESTMODE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using InfoReader.Command;
using InfoReader.Command.Parser;
using InfoReader.Configuration;
using InfoReader.Configuration.Attributes;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Mmf;
using InfoReader.Tools;
using InfoReader.Tools.I8n;
using OsuRTDataProvider;
using osuTools;
using osuTools.OrtdpWrapper;
using RealTimePPDisplayer;
using Sync.Command;
using Sync.Plugins;
using Logger = InfoReader.Tools.Logger;

namespace InfoReader
{
    [SyncSoftRequirePlugin("RealTimePPDisplayerPlugin", "OsuRTDataProviderPlugin")]
    public class InfoReaderPlugin: Plugin
    {
        private static readonly Dictionary<Type, IConfigConverter> Converters = new();

        internal readonly Dictionary<string, IConfigurable> Configurables = new();
        internal readonly Dictionary<int, IConfigElement> ConfigElements = new();
        private readonly Dictionary<string, ICommandProcessor> _commandProcessors = new();

        

        internal void InitCommand(Dictionary<Type,object[]>? commandTypeArgs = null)
        {
            Type[] commandTypes =
                ReflectionTools.GetTypesWithInterface<ICommandProcessor>(Assembly.GetExecutingAssembly());
            foreach (Type commandType in commandTypes)
            {
                var args = Array.Empty<object>();
                if (commandTypeArgs != null && commandTypeArgs.ContainsKey(commandType))
                {
                    args = commandTypeArgs[commandType];
                }
                ICommandProcessor? commandProcessor = (ICommandProcessor?) ReflectionTools.CreateInstance(commandType, args);
                if (commandProcessor == null)
                    continue;
                _commandProcessors.Add(commandProcessor.MainCommand, commandProcessor);
            }
        }

        internal void ScanConfigurations(Dictionary<Type, object[]>? commandTypeArgs = null)
        {
            var types = ReflectionTools.GetTypesWithInterface<IConfigurable>(Assembly.GetExecutingAssembly());
            foreach (var type in types)
            {
                object[] args = Array.Empty<object>();
                if (commandTypeArgs != null && commandTypeArgs.ContainsKey(type))
                {
                    args = commandTypeArgs[type];
                }                    //ReflectionTools.CreateInstance<IConfigurable>(args);
                var configuration = (IConfigurable?)ReflectionTools.CreateInstance(type, args);
                if (configuration == null)
                {
                    continue;
                }
                Configurables.Add(configuration.ConfigArgName, configuration);
            }
        }

        private System.Timers.Timer _configUpdateTimer = new(500);
        public InfoReaderConfiguration Configuration { get; } = new();
        public MmfConfiguration MmfConfiguration { get; } = new();
        public IMemoryDataSource? MemoryDataSource { get; private set; }
        public IConfigElement ConfigElement { get; internal set; }

        public Dictionary<string, PropertyInfo> Variables { get; private set; } = new();
        public InfoReaderPlugin() : base("InfoReader", "Someone999")
        {
            string syncPath = Process.GetCurrentProcess().MainModule.FileName;
            string currentDir = ".\\InfoReader\\";
            Environment.CurrentDirectory = currentDir;
            var configElement = new TomlConfigElement("InfoReaderConfig.toml");
            ConfigElement = configElement;
            Dictionary<Type, object?[]> converterTypesArgs = new Dictionary<Type, object?[]>
            {
                {typeof(MmfListConverter), new object[] {this}}
            };
            ScanConfigurations();
            ConfigTools.ReadConfigFile(Configurables.Values.ToArray(), converterTypesArgs);
            InitCommand();
            LocalizationInfo.Current = LocalizationInfo.GetLocalizationInfo(Configuration.LanguageId);
            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(Loaded);
            EventBus.BindEvent<PluginEvents.InitCommandEvent>(InitSyncCommand);
            ThreadPool.QueueUserWorkItem(state =>
                Variables = VariableTools.GetAvailableVariables(typeof(OrtdpWrapper)));
        }

        bool CommandProcessor(Arguments args)
        {
            ICommandProcessor? processor = null;
            try
            {
                CommandParser parser = new CommandParser(args.ToArray());
                if (_commandProcessors.ContainsKey(parser.MainCommand))
                {
                    processor = _commandProcessors[parser.MainCommand];
                }

                if (processor == null)
                {
                    Logger.LogError("[InfoReader] No such command.");
                    return true;
                }
                bool suc = processor.Execute(this, parser);
                if (!suc)
                {
                    Logger.LogError(processor.GetHelp());
                }
                return true;
            }
            catch (Exception e)
            {
                if (processor == null)
                {
                    Console.WriteLine(e.Message);
                    return true;
                }
                if (!processor.AutoCatch)
                    throw new Exception(LocalizationInfo.Current.Translations["LANG_ERR_PROCESSOREXCEPTION"]);
                bool handled = processor.OnUnhandledException(this, e);
                if (!handled)
                {
                    string notification = string.Format(LocalizationInfo.Current.Translations["LANG_ERR_UNHANDLEDEXCEPTION"],
                        processor.MainCommand, e);
                    Tools.Logger.LogError(notification);
                }

                return true;
            }
        }

        void InitSyncCommand(PluginEvents.InitCommandEvent e)
        {
            e.Commands.Dispatch.bind("getinfo", CommandProcessor, "None");
        }

        void Loaded(PluginEvents.LoadCompleteEvent e)
        {
            var ortdpPlugin = getHoster().EnumPluings().First(p => p.Name == "OsuRTDataProvider") as OsuRTDataProviderPlugin;
            var rtppdPlugin = getHoster().EnumPluings().First(p => p.Name == "RealTimePPDisplayer") as RealTimePPDisplayerPlugin;
            RtppdInfo rtppd = new RtppdInfo();
            MemoryDataSource = new OrtdpWrapper(ortdpPlugin, rtppdPlugin, rtppd);
            MmfManager.GetInstance(this).StartUpdate(100);
        }


    }
}
