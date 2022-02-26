#define TESTMODE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
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
        internal readonly Dictionary<string, IConfigurable> Configurables = new();
        internal readonly Dictionary<string, IConfigElement> ConfigElements = new();
        internal readonly Dictionary<string, ICommandProcessor> CommandProcessors = new();

        

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
                CommandProcessors.Add(commandProcessor.MainCommand, commandProcessor);
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

        public InfoReaderConfiguration Configuration { get; }
        public MmfConfiguration MmfConfiguration { get; }
        public IMemoryDataSource? MemoryDataSource { get; private set; }

        public Dictionary<string, PropertyInfo> Variables { get; private set; } = new();
        public InfoReaderPlugin() : base("InfoReader", "Someone999")
        {
            string syncPath = Process.GetCurrentProcess().MainModule.FileName;
            string currentDir = ".\\InfoReader\\";
            Environment.CurrentDirectory = currentDir;
            var configElement = new TomlConfigElement("InfoReaderConfig.toml");
            Dictionary<Type, object?[]> converterTypesArgs = new Dictionary<Type, object?[]>
            {
                {typeof(MmfListConverter), new object[] {this}}
            };
            ScanConfigurations();
            ConfigTools.ReadConfigFile(Configurables.Values.ToArray(), converterTypesArgs, ConfigElements);
            Configuration = (InfoReaderConfiguration)Configurables["program"];
            MmfConfiguration = (MmfConfiguration) Configurables["mmf"];
            InitCommand();
            LocalizationInfo.Current = LocalizationInfo.GetLocalizationInfo(Configuration.LanguageId);
            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(Loaded);
            EventBus.BindEvent<PluginEvents.InitCommandEvent>(InitSyncCommand);
            ThreadPool.QueueUserWorkItem(state =>
                Variables = VariableTools.GetAvailableVariables(typeof(OrtdpWrapper)));
            ThreadPool.QueueUserWorkItem(state => WaitingProcess());
        }

        private void WaitingProcess()
        {
            Process[] processes;
            while ((processes = ProcessTools.FindProcess("osu!")).Length == 0)
            {
                Thread.Sleep(3000);
            }

            if (processes.Length <= 0)
                return;
            string? dir = processes[0].MainModule.FileName;
            if (string.IsNullOrEmpty(dir))
                return;
            Configuration.GameDirectory = dir;
        }

        internal bool CommandProcessor(Arguments args)
        {
            ICommandProcessor? processor = null;
            try
            {
                CommandParser parser = new CommandParser(args.ToArray());
                if (CommandProcessors.ContainsKey(parser.MainCommand))
                {
                    processor = CommandProcessors[parser.MainCommand];
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

        private void InitSyncCommand(PluginEvents.InitCommandEvent e)
        {
            e.Commands.Dispatch.bind("getinfo", CommandProcessor, "None");
        }

        private void Loaded(PluginEvents.LoadCompleteEvent e)
        {
            var ortdpPlugin = getHoster().EnumPluings().First(p => p.Name == "OsuRTDataProvider") as OsuRTDataProviderPlugin;
            var rtppdPlugin = getHoster().EnumPluings().First(p => p.Name == "RealTimePPDisplayer") as RealTimePPDisplayerPlugin;
            RtppdInfo rtppd = new RtppdInfo();
            MemoryDataSource = new OrtdpWrapper(ortdpPlugin, rtppdPlugin, rtppd);
            MmfManager.GetInstance(this).StartUpdate(100);
        }

        public override void OnDisable()
        {
            Dictionary<Type, object?[]> converterTypesArgs = new Dictionary<Type, object?[]>
            {
                {typeof(MmfListConverter), new object[] {this}}
            };
            foreach (var configurable in Configurables)
            {
                configurable.Value.Save(converterTypesArgs);
            }

            foreach (var configElement in ConfigElements)
            {
                var cfgFile = configElement.Key;
                File.Copy(cfgFile, $"ConfigBackup\\{cfgFile}{DateTime.Now.Ticks}.bak");
                if (configElement.Value is IConfigWriter writer)
                {
                    writer.WriteToFile(cfgFile);
                }
            }
            
        }
    }
}
