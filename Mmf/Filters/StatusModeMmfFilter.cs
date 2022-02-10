using System;
using System.Collections.Generic;
using osuTools.Game;
using osuTools.Game.Modes;

namespace InfoReader.Mmf.Filters;

public class StatusModeMmfFilter : IMmfFilter
{
    public string MmfType => "StatusModeMmf";

    public static OsuGameMode ModeProcessor(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return OsuGameMode.Unknown;
        }

        input = input.ToLower();
        return input switch
        {
            "osu" => OsuGameMode.Osu,
            "std" => OsuGameMode.Osu,
            "taiko" => OsuGameMode.Taiko,
            "ctb" => OsuGameMode.Catch,
            "catch" => OsuGameMode.Catch,
            "mania" => OsuGameMode.Mania,
            "0" => OsuGameMode.Osu,
            "1" => OsuGameMode.Taiko,
            "2" => OsuGameMode.Catch,
            "3" => OsuGameMode.Mania,
            _ => OsuGameMode.Unknown
        };
    }

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
            status |= (OsuGameStatus)Enum.Parse(typeof(OsuGameStatus), stat);
        }

        return status;
    }
    public MmfBase Filter(Dictionary<string, object> config)
    {
        string name = config["Name"].ToString();
        StatusModeMmf mmf = new StatusModeMmf(name, ModeProcessor(config["Mode"].ToString()),
            StatusProcessor(config["Status"].ToString()))
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