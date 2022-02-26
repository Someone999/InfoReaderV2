using System.Net.Http;
using Newtonsoft.Json;

namespace InfoReader.Update;

internal static class InfoReaderUpdateTools
{
    internal static PluginFile[] GetUpdateFiles(string version)
    {
        HttpClient client = new HttpClient();
        var url = string.Format(InfoReadUrl.GetFilesUrl, version);
        var rslt = client.GetStringAsync(url).Result;
        var deserialized = JsonConvert.DeserializeObject<PluginUpdateFilesInfo>(rslt) ?? new PluginUpdateFilesInfo();
        return deserialized.Files;
    }

    internal static AvailabilityStatus GetAvailabilityStatus(string version)
    {
        HttpClient client = new HttpClient();
        var url = string.Format(InfoReadUrl.AvailabilityStatusUrl, version);
        var rslt = client.GetStringAsync(url).Result;
        var deserialized = JsonConvert.DeserializeObject<PluginAvailabilityInfo>(rslt);
        return deserialized?.AvailabilityStatus ?? AvailabilityStatus.None;
    }
}