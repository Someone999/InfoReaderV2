#define TESTMODE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using InfoReader.Attributes;
using InfoReader.Command;
using InfoReader.Command.Parser;
using InfoReader.Configuration;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Exceptions;
using InfoReader.Mmf;
using InfoReader.Resource;
using InfoReader.Tools;
using InfoReader.Tools.I8n;
using InfoReader.Update;
using InfoReader.Version;
using OsuRTDataProvider;
using osuTools;
using osuTools.MD5Tools;
using osuTools.OrtdpWrapper;
using RealTimePPDisplayer;
using Sync.Command;
using Sync.Plugins;
using Logger = InfoReader.Tools.Logger;

namespace InfoReader
{
    [InfoReaderVersion("1.0.21")]
    [SyncSoftRequirePlugin("RealTimePPDisplayerPlugin", "OsuRTDataProviderPlugin")]
    public class InfoReaderPlugin : Plugin
    {
        internal readonly Dictionary<string, IConfigurable> Configurables = new();
        internal readonly Dictionary<string, IConfigElement> ConfigElements = new();
        internal readonly Dictionary<string, ICommandProcessor> CommandProcessors = new();
        internal IResourceManager ResourceManager { get; set; }

        public PluginVersion? GetCurrentVersion()
        {
            Type type = typeof(InfoReaderPlugin);
            var attrs = type.GetCustomAttributes<InfoReaderVersionAttribute>();
            return attrs.ElementAt(0).PluginVersion;
        }


        internal void InitCommand(Dictionary<Type, object[]>? commandTypeArgs = null)
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
                ICommandProcessor? commandProcessor = (ICommandProcessor?)ReflectionTools.CreateInstance(commandType, args);
                if (commandProcessor == null)
                    continue;
                CommandProcessors.Add(commandProcessor.MainCommand, commandProcessor);
            }
        }

        internal void ScanConfigurations(Dictionary<Type, object?[]>? commandTypeArgs = null)
        {
            var types = ReflectionTools.GetTypesWithInterface<IConfigurable>(Assembly.GetExecutingAssembly());
            foreach (var type in types)
            {
                object?[] args = Array.Empty<object>();
                if (commandTypeArgs != null && commandTypeArgs.ContainsKey(type))
                {
                    args = commandTypeArgs[type];
                }                    //ReflectionTools.CreateInstance<IConfigurable>(args);
                var configuration = (IConfigurable?)ReflectionTools.CreateInstance(type, args);
                if (configuration == null)
                {
                    continue;
                }
                Configurables.Add(configuration.ConfigName, configuration);
            }
        }

        public InfoReaderConfiguration Configuration { get; }
        public MmfConfiguration MmfConfiguration { get;  }
        public IMemoryDataSource? MemoryDataSource { get; private set; }

        public Dictionary<string, PropertyInfo> Variables { get; private set; } = new();
        public Dictionary<string, PropertyInfo> LowerCasedVariables { get; } = new();
        public InfoReaderPlugin() : base("InfoReader", "Someone999")
        {
            /*AppDomain.CurrentDomain.Load(Resource1.Newtonsoft_Json);
            var s = ReflectAssemblies.NewtonsoftJson.Assembly;*/
            string resFilePath = "InfoReaderResources.ifrresc";
            FileTools.ConfirmDirectory("InfoReader\\");
            string currentDir = ".\\InfoReader\\";
            string realResFilePath = Path.Combine(currentDir, resFilePath);
            if (File.Exists(resFilePath) && !File.Exists(realResFilePath))
            {
                File.Copy(resFilePath, realResFilePath);
            }
            
            Environment.CurrentDirectory = currentDir;
            ResourceManager = ResourceManager<CompressedResourceContainerReader, CompressedResourceContainerWriter>.GetInstance
            (resFilePath,
                new Dictionary<Type, object?[]?>
                {
                    {typeof(CompressedResourceContainerReader),new object?[]{resFilePath, true}},
                    {typeof(CompressedResourceContainerWriter),new object?[]{resFilePath, true}}
                });
            CheckFiles();
            var configElement = new TomlConfigElement("InfoReaderConfig.toml");
            InitConfiguration();
            Configuration = (InfoReaderConfiguration)Configurables["program"];
            MmfConfiguration = (MmfConfiguration)Configurables["mmf"];
            CheckMigrate(configElement);
            InitCommand();
            CheckConfigFileBackup();
            LocalizationInfo.Current = LocalizationInfo.GetLocalizationInfo(Configuration.LanguageId);
            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(Loaded);
            EventBus.BindEvent<PluginEvents.InitCommandEvent>(InitSyncCommand);
            ThreadPool.QueueUserWorkItem(state =>
            {
                Variables = VariableTools.GetAvailableVariables(typeof(OrtdpWrapper));
                foreach (var varInfo in Variables)
                {
                    string lowerName = varInfo.Key.ToLowerInvariant();
                    if (LowerCasedVariables.ContainsKey(lowerName))
                        continue;
                    LowerCasedVariables.Add(lowerName, varInfo.Value);
                }
            });

            try
            {
                PluginVersion latestVersion = PluginVersion.LatestVersion;
                Logger.LogNotification(LocalizationInfo.Current.Translations["LANG_INFO_CHECKUPDATE"]);
                if ((GetCurrentVersion() ?? PluginVersion.InvalidVersion) < latestVersion)
                {
                    //Update disabled yet.
                    Task.Run(() => new Updater().Update(latestVersion));
                }
            }
            catch (Exception)
            {
                Logger.LogNotification(LocalizationInfo.Current.Translations["LANG_ERR_NETWORK"]);
            }

            ThreadPool.QueueUserWorkItem(state => WaitingProcess());
        }

        private void CheckFiles()
        {
            bool needRestart = false;
            ResourceFileInfo[] resources = ResourceManager.ResourceContainerReader.GetResources();
            foreach (var resourceFileInfo in resources)
            {
                if (resourceFileInfo.Exists())
                    continue;
                Logger.LogNotification("Lacked file has been released.");
                resourceFileInfo.WriteToFile(resourceFileInfo.ResourcePath);
                needRestart = true;
            }

            Assembly currentAsm = Assembly.GetExecutingAssembly();
            string? asmDir = Path.GetDirectoryName(currentAsm.Location);
            if (string.IsNullOrEmpty(asmDir))
            {
                asmDir = ".";
            }

            string sqliteDllDir = Path.Combine(asmDir, "x86\\");
            if (Directory.Exists(sqliteDllDir)) 
                return;
            FileTools.ConfirmDirectory(sqliteDllDir);
            File.Copy("x86\\SQLite.Interop.dll", Path.Combine(sqliteDllDir, "SQLite.Interop.dll"));
            if (needRestart)
            {
                getHoster()?.RestartSync();
            }
        }

        private void CheckConfigFileBackup()
        {
            var files = Array.Empty<string>();
            if (Directory.Exists("ConfigBackup\\"))
            {
                files = Directory.GetFiles("ConfigBackup\\");
            }

            if (files.Length == 0)
                return;
            TimeSpan aWeek = TimeSpan.FromDays(7);
            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (!fileInfo.Exists)
                {
                    continue;
                }
                DateTime now = DateTime.Now.ToUniversalTime();

                if (now - fileInfo.CreationTimeUtc > aWeek)
                {
                    fileInfo.Delete();
                }
            }

        }

        private void CheckMigrate(IConfigElement configElement)
        {
            if (!configElement["Migration"]["IsMigrated"].GetValue<bool>())
            {
                new MigrationAssistant(this).Migrate();
            }
        }

        private void InitConfiguration()
        {
            Dictionary<Type, object?[]> converterTypesArgs = new Dictionary<Type, object?[]>
            {
                {typeof(MmfListConverter), new object[] {this}}
            };
            Dictionary<Type, object?[]> commandTypeArgs = new Dictionary<Type, object?[]>
            {
                {typeof(InfoReaderConfiguration), new object[]{this}}
            };
            ScanConfigurations(commandTypeArgs);
            ConfigTools.ReadConfigFile(Configurables.Values.ToArray(), converterTypesArgs, ConfigElements);
            
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
            Configuration.GamePath = dir;
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
                    throw new CommandInvocationException(LocalizationInfo.Current.Translations["LANG_ERR_PROCESSOREXCEPTION"]);
                bool handled = processor.OnUnhandledException(this, e);
                if (!handled)
                {
                    string notification = string.Format(LocalizationInfo.Current.Translations["LANG_ERR_UNHANDLEDEXCEPTION"],
                        processor.MainCommand, e);
                    Logger.LogError(notification);
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

        public override void OnExit()
        {
            OnDisable();
            base.OnExit();
        }
    }
}
