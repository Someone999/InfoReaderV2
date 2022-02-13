using System;
using System.Collections.Generic;
using osuTools.Game;
using osuTools.Game.Modes;

namespace InfoReader.Mmf.Filters;

public class StatusModeMmfFilter : IMmfFilter
{
    public string MmfType => "StatusModeMmf";
    public MmfBase Filter(Dictionary<string, object> config)
    {
        string name = config["Name"].ToString();
        StatusModeMmf mmf = new StatusModeMmf(name, ModeMmfFilter.MulGameModeProcessor(config["Mode"].ToString()),
            StatusMmfFilter.StatusProcessor(config["Status"].ToString()))
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