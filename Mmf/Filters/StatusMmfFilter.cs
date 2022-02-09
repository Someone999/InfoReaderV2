using System;
using System.Collections.Generic;
using osuTools.Game;

namespace InfoReader.Mmf.Filters;

public class StatusMmfFilter: IMmfFilter
{
    public string MmfType => "StatusMmf";

    public static OsuGameStatus StatusProcessor(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return OsuGameStatus.Unkonwn;
        }

        string[] stats = input.Split(',');
        OsuGameStatus status = 0;
        foreach (var stat in stats)
        {
            status |= (OsuGameStatus) Enum.Parse(typeof(OsuGameStatus), stat);
        }

        return status;
    }
    public MmfBase Filter(Dictionary<string, object> config)
    {
        string name = config["Name"].ToString();
        
        StatusMmf mmf = new StatusMmf(name, StatusProcessor(config["Status"].ToString()))
        {
            FormatFile = config["FormatFile"].ToString(),
            Enabled = bool.Parse(config["Enabled"].ToString())
        };
        return mmf;
    }
}