using System.Collections.Generic;
using osuTools.Game.Modes;

namespace InfoReader.Mmf.Filters;

public class ModeMmfFilter : IMmfFilter
{
    public string MmfType => "ModeMmf";

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
    public MmfBase Filter(Dictionary<string, object> config)
    {
        string name = config["Name"].ToString();
        ModeMmf mmf = new ModeMmf(name, ModeProcessor(config["Mode"].ToString()))
        {
            FormatFile = config["FormatFile"].ToString(),
            Enabled = bool.Parse(config["Enabled"].ToString())
        };
        return mmf;
    }
}