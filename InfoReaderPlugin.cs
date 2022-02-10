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
using InfoReader.Configuration.Attribute;
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
        static readonly Dictionary<Type, IConfigConverter> _converters = new();
        private readonly Dictionary<string, ICommandProcessor> _commandProcessors = new();
        public static void InitConfig(IConfigurable configurable, IConfigElement element, Dictionary<Type,object[]>? converterTypeArgs = null)
        {
            if (configurable == null)
            {
                throw new ArgumentNullException(nameof(configurable));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }


            var props = ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>
                (configurable.GetType(), BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in props)
            {
                IConfigElement lastElement = element;
                var cfgInfo = property.Item2[0];
                string[] paths = string.IsNullOrEmpty(cfgInfo.ConfigPath)
                    ? new[] { property.Item1.Name }
                    : cfgInfo.ConfigPath.Split('.');
                lastElement = paths.Aggregate(lastElement, (current, path) => current[path]);

                Type? converter = cfgInfo.ConverterType;
                object? cfgVal;
                if (converter == null)
                {
                    cfgVal = lastElement.GetValue(property.Item1.PropertyType);
                }
                else
                {
                    if (!_converters.ContainsKey(converter))
                    {
                        var args = Array.Empty<object>();
                        if (converterTypeArgs != null && converterTypeArgs.ContainsKey(converter))
                        {
                            args = converterTypeArgs[converter];
                        }
                        var converterIns =
                            (IConfigConverter?)ReflectionTools.CreateInstance(converter, args);
                        _converters.Add(converter, converterIns ?? throw new InvalidOperationException());
                    }

                    cfgVal = lastElement.GetValue(_converters[converter]);
                }

                property.Item1.SetValue(configurable, cfgVal);
            }
        }

        internal void InitCommand(Dictionary<Type,object[]>? commandTypeArgs = null)
        {
            Type[] commandTypes =
                ReflectionTools.GetTypesWithInterface(Assembly.GetExecutingAssembly(), nameof(ICommandProcessor));
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

        private System.Timers.Timer _configUpdateTimer = new(500);
        public InfoReaderConfiguration Configuration { get; } = new();
        public MmfConfiguration MmfConfiguration { get; } = new();
        public IMemoryDataSource? MemoryDataSource { get; private set; }

        public Dictionary<string, PropertyInfo> Variables { get; private set; } = new();
        public InfoReaderPlugin() : base("InfoReader", "Someone999")
        {
            string syncPath = Process.GetCurrentProcess().MainModule.FileName;
            string currentDir = ".\\InfoReader\\";
            Environment.CurrentDirectory = currentDir;
            var configElement = new TomlConfigElement("InfoReaderConfig.toml");
            Dictionary<Type, object[]> converterTypesArgs = new Dictionary<Type, object[]>
            {
                {typeof(MmfsConverter), new object[] {this}},
                {typeof(BeatmapReadMethodsConverter), new object[]{this}}
            };
            InitConfig(Configuration, configElement, converterTypesArgs);
            InitConfig(MmfConfiguration, configElement, converterTypesArgs);
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
