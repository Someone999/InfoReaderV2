using System;
using System.Collections.Generic;
using System.Linq;
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

        string[] stats = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        return stats.Aggregate<string?, OsuGameStatus>(0, (current, stat) => current | (OsuGameStatus) Enum.Parse(typeof(OsuGameStatus), stat));
    }

    public MmfBase Filter(Dictionary<string, object> config)
    {
        string name = config["Name"].ToString();
        
        StatusMmf mmf = new StatusMmf(name, StatusProcessor(config["EnabledStatus"].ToString()))
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