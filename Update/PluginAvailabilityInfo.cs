using Newtonsoft.Json;

namespace InfoReader.Update;

internal class PluginAvailabilityInfo
{
    [JsonProperty("version")] 
    public string Version { get; set; } = "";
    [JsonProperty("availability")]
    public AvailabilityStatus AvailabilityStatus { get; set; }

}