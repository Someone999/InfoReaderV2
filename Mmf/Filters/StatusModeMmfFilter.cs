using System.Collections.Generic;

namespace InfoReader.Mmf.Filters;

public class StatusModeMmfFilter : IMmfFilter
{
    public string MmfType => "StatusModeMmf";
    public MmfBase Filter(Dictionary<string, object> config)
    {
        string name = config["Name"].ToString();
        StatusModeMmf mmf = new StatusModeMmf(name, ModeMmfFilter.MulGameModeProcessor(config["EnabledMode"].ToString()),
            StatusMmfFilter.StatusProcessor(config["EnabledStatus"].ToString()))
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