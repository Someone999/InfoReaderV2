using InfoReader.Configuration.Attribute;

namespace InfoReader.Configuration
{ /*
BeatmapCopyDirectory: .\Beatmap\Beatmap
AudioCopyDirectory: .\Beatmap\Audio
VedioCopyDirectory: .\Beatmap\Vedio
BackgroundCopyDirectory: .\Beatmap\Background
#Ortdp or osuTools
BeatmapReadMethod: osuTools
#You'd better don't change it.
MmfEncoding: UTF-8*/
    public class InfoReaderConfiguration : IConfigurable
    {
        [ConfigItem] 
        public string LanguageId { get; set; } = "en-us";
        [ConfigItem]
        public bool DebugMode { get; set; }
        [ConfigItem]
        public string? OsuApiKey { get; set; }
        [ConfigItem]
        public string? BeatmapCopyDirectory { get; set; }
        [ConfigItem]
        public string? AudioCopyDirectory { get; set; }
        [ConfigItem]
        public string? VideoCopyDirectory { get; set; }

        public void Save()
        {
            YamlDotNet.Serialization.Serializer serializer = new YamlDotNet.Serialization.Serializer();
            var serialized = serializer.Serialize(this);
            //File.WriteAllText();
        }
    }
}
