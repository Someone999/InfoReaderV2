using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using InfoReader.Configuration.Attributes;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Configuration.Gui;
using InfoReader.Tools;
using osuTools.OrtdpWrapper;

namespace InfoReader.Configuration
{
    public class InfoReaderConfiguration : IGuiConfigurable
    {
        public string ConfigFilePath => DefaultFilePath.CurrentConfigFile;
        public Type ConfigElementType => typeof(TomlConfigElement);
        public string ConfigArgName => "program";
        [ConfigItem("Program.LanguageId")] 
        public string LanguageId { get; set; } = "en-us";

        [Bool]
        [ConfigItem("Program.DebugMode")]
        public bool DebugMode { get; set; }

        [ConfigItem("Program.OsuApiKey")] 
        public string OsuApiKey { get; set; } = "";

        [ConfigItem("Program.BeatmapCopyDirectory")]
        public string BeatmapCopyDirectory { get; set; } = ".\\Beatmap\\Beatmap";

        [ConfigItem("Program.AudioCopyDirectory")]
        public string AudioCopyDirectory { get; set; } = ".\\Beatmap\\Audio";

        [ConfigItem("Program.BackgroundCopyDirectory")]
        public string BackgroundCopyDirectory { get; set; } = ".\\Beatmap\\Background";

        [ConfigItem("Program.VideoCopyDirectory")]
        public string VideoCopyDirectory { get; set; } = ".\\Beatmap\\Video";
        [List("OsuDb","OsuDb", "Ortdp")]
        [ConfigItem("Program.BeatmapReadMethod", converterType: typeof(BeatmapReadMethodsConverter))]
        public OrtdpWrapper.BeatmapReadMethods ReadMethod { get; set; }

        

        public void Save(IConfigElement element, Dictionary<Type,object?[]>? typeConverterArgs = null)
        {
            Type cfgType = typeof(MmfConfiguration);
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public;
            foreach (var propertyInfo in ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>(cfgType, bindingAttr))
            {
                IConfigElement tmp = element;
                var cfgInfo = propertyInfo.Item2[0];
                string[] parts = cfgInfo.ConfigPath.Split('.');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    tmp = tmp[parts[i]];
                }

                object? currentValue = propertyInfo.Item1.GetValue(this);
                if (cfgInfo.ConverterType != null)
                {
                    object?[] args = Array.Empty<object>();
                    if (typeConverterArgs != null && typeConverterArgs.ContainsKey(cfgInfo.ConverterType))
                    {
                        args = typeConverterArgs[cfgInfo.ConverterType];
                    }
                    currentValue = (IConfigConverter?)ReflectionTools.CreateInstance(cfgInfo.ConverterType, args);
                }

                tmp.SetValue(parts.Last(), currentValue);
            }
        }

        public Form CreateConfigWindow()
        {
            Form f = new Form();
            return f;
        }

        void AddControls(Form form)
        {

        }
    }
}
