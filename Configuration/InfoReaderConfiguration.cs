using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfoReader.Configuration.Attributes;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Configuration.Gui;
using InfoReader.Tools;
using InfoReader.Tools.I8n;
using osuTools.OrtdpWrapper;

namespace InfoReader.Configuration
{
    public class InfoReaderConfiguration : IGuiConfigurable
    {
        public InfoReaderConfiguration(InfoReaderPlugin plugin)
        {
            _plugin = plugin;
        }

        public string ConfigFilePath => DefaultFilePath.CurrentConfigFile;
        public Type ConfigElementType => typeof(TomlConfigElement);
        public string ConfigName => "program";
        public IConfigElement? ConfigElement { get; set; }
        private string _langId = "en-us";
        private InfoReaderPlugin _plugin;

        [ConfigItem("Program.LanguageId", "L::LANG_CFG_LANGUAGEID")]
        public string LanguageId
        {
            get => _langId;
            set
            {
                if (value.Length < 5) 
                    return;
                LocalizationInfo.Current = LocalizationInfo.GetLocalizationInfo(value);
                _langId = value;
            }
        }

        [ConfigItem("Program.DebugMode", "L::LANG_CFG_DEBUGMODE")]
        public bool DebugMode
        {
            get
            {
                while (_plugin.MemoryDataSource is null)
                {
                    Thread.Sleep(1);
                }
                Thread.Sleep(10);
                var wrapper = (OrtdpWrapper)_plugin.MemoryDataSource;
                lock (wrapper)
                {
                    return wrapper.DebugMode;
                }
            }
            set
            {
                Task.Run(() =>
                {
                    while (_plugin.MemoryDataSource is null)
                    {
                        Thread.Sleep(1);
                    }
                    
                    var wrapper = (OrtdpWrapper) _plugin.MemoryDataSource;
                    lock (wrapper)
                    {
                        wrapper.DebugMode = value;
                    }

                });
            }
        }

        [ConfigItem("Program.OsuApiKey", "L::LANG_CFG_APIKEY")] 
        public string OsuApiKey { get; set; } = "";
        [ConfigItem("Program.OverrideFile","L::LANG_CFG_OVERRIDE_EXISTED_FILE")]
        public bool OverrideExistedFile { get; set; }

        [ConfigItem("Program.BeatmapCopyDirectory", "L::LANG_CFG_BEATMAPDIR")]
        public string BeatmapCopyDirectory { get; set; } = ".\\Beatmap\\Beatmap";

        [ConfigItem("Program.AudioCopyDirectory", "L::LANG_CFG_AUDIODIR")]
        public string AudioCopyDirectory { get; set; } = ".\\Beatmap\\Audio";

        [ConfigItem("Program.BackgroundCopyDirectory", "L::LANG_CFG_BGDIR")]
        public string BackgroundCopyDirectory { get; set; } = ".\\Beatmap\\Background";

        [ConfigItem("Program.VideoCopyDirectory", "L::LANG_CFG_VIDEODIR")]
        public string VideoCopyDirectory { get; set; } = ".\\Beatmap\\Video";
        [ConfigItem("Program.GamePath", "L::LANG_CFG_GAMEDIR")]
        public string GamePath { get; set; } = "";

        [List("OsuDb", "OsuDb", "Ortdp")]
        [ConfigItem("Program.BeatmapReadMethod", "L::LANG_CFG_READMETHOD", typeof(BeatmapReadMethodsConverter))]
        public OrtdpWrapper.BeatmapReadMethods ReadMethod
        {
            get
            {
                while (_plugin.MemoryDataSource is null)
                {
                    Thread.Sleep(1);
                }
                Thread.Sleep(10);
                var wrapper = (OrtdpWrapper)_plugin.MemoryDataSource;
                lock (wrapper)
                {
                    return wrapper.BeatmapReadMethod;
                }
            }
            set
            {

                Task.Run(() =>
                {
                    while (_plugin.MemoryDataSource is null)
                    {
                        Thread.Sleep(1);
                    }

                    var wrapper = (OrtdpWrapper) _plugin.MemoryDataSource;
                    lock (wrapper)
                    {
                        wrapper.BeatmapReadMethod = value;
                    }
                    
                });
            }
        }

        [ConfigItem("Program.AutoUpdate", "L::LANG_CFG_AUTOUPDATE")]
        public bool AutoUpdate { get; set; } = true;

        public void Save(Dictionary<Type,object?[]>? typeConverterArgs = null)
        {
            Type cfgType = typeof(InfoReaderConfiguration);
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public;
            foreach (var propertyInfo in ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>(cfgType, bindingAttr))
            {
                IConfigElement tmp = ConfigElement ?? throw new InvalidOperationException();
                var cfgInfo = propertyInfo.Item2[0];
                string[] parts = cfgInfo.ConfigPath.Split('.');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    tmp = tmp[parts[i]];
                }

                if (GetType() != propertyInfo.Item1.DeclaringType)
                {
                    return;
                }
                object? currentValue = propertyInfo.Item1.GetValue(this);
                if (cfgInfo.ConverterType != null)
                {
                    object?[] args = Array.Empty<object>();
                    if (typeConverterArgs != null && typeConverterArgs.ContainsKey(cfgInfo.ConverterType))
                    {
                        args = typeConverterArgs[cfgInfo.ConverterType];
                    }
                    var converter = (IConfigConverter?)ReflectionTools.CreateInstance(cfgInfo.ConverterType, args);
                    currentValue = converter?.ToDictionary(currentValue) ?? converter?.ToValue(currentValue) ?? throw new ArgumentNullException();
                }

                tmp.SetValue(parts.Last(), currentValue);
            }
        }

        public Form CreateConfigWindow()
        {
            return WindowTools.CreateDefaultConfigForm(this);
        }
    }
}
