using System.Collections.Generic;

namespace InfoReader.Mmf.Filters;

public class DefaultMmfFilter : IMmfFilter
{
    public string MmfType => "NormalMmf";
    public MmfBase Filter(Dictionary<string, object> config)
    {
        NormalMmf mmf = new NormalMmf(config["Name"].ToString())
        {
            FormatFile = config["FormatFile"].ToString(),
            Enabled = bool.Parse(config["Enabled"].ToString())
        };
        if (config.ContainsKey("UpdateInterval"))
        {
            mmf.UpdateInterval = int.Parse(config["UpdateInterval"].ToString());
        }

        return mmf;
    }
}