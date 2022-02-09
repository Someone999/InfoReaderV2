using System;
using System.Collections.Generic;
using InfoReader.Configuration.Attribute;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using osuTools.OrtdpWrapper;

namespace InfoReader.Configuration
{
    public class InfoReaderConfiguration : IConfigurable
    {
        [ConfigItem("Program.LanguageId")] 
        public string LanguageId { get; set; } = "en-us";
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

        [ConfigItem("Program.BeatmapReadMethod", converterType: typeof(BeatmapReadMethodsConverter))]
        public OrtdpWrapper.BeatmapReadMethods ReadMethod { get; set; }
        
        public void Save(IConfigElement element, Dictionary<Type,object?[]>? typeConverterArgs = null)
        {
            YamlDotNet.Serialization.Serializer serializer = new YamlDotNet.Serialization.Serializer();
            var serialized = serializer.Serialize(this);
            //File.WriteAllText();
        }
    }
}
