using System;
using Newtonsoft.Json;

namespace InfoReader.Update;

internal class PluginUpdateFilesInfo
{
    [JsonProperty("count")]
    internal int Count { get; set; }
    [JsonProperty("files")]
    internal PluginFile[] Files { get; set; } = Array.Empty<PluginFile>();
        
}