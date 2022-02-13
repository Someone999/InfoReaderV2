using System;
using System.Collections.Generic;
using osuTools.Game.Modes;

namespace InfoReader.Mmf.Filters;

public class ModeMmfFilter : IMmfFilter
{
    public string MmfType => "ModeMmf";

    public static MmfGameMode ModeProcessor(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return MmfGameMode.Unknown;
        }

        input = input.ToLower();
        return input switch
        {
            "osu" => MmfGameMode.Osu,
            "std" => MmfGameMode.Osu,
            "taiko" => MmfGameMode.Taiko,
            "ctb" => MmfGameMode.Catch,
            "catch" => MmfGameMode.Catch,
            "mania" => MmfGameMode.Mania,
            "0" => MmfGameMode.Osu,
            "1" => MmfGameMode.Taiko,
            "2" => MmfGameMode.Catch,
            "3" => MmfGameMode.Mania,
            _ => MmfGameMode.Unknown
        };
    }

    public static MmfGameMode MulGameModeProcessor(string input)
    {
        MmfGameMode baseMode = 0;
        string[] modes = input.Split(new []{','} , StringSplitOptions.RemoveEmptyEntries);
        foreach (var mode in modes)
        {
            baseMode |= ModeProcessor(mode);
        }
        return baseMode;
    }

    public MmfBase Filter(Dictionary<string, object> config)
    {
        string name = config["Name"].ToString();
        ModeMmf mmf = new ModeMmf(name, MulGameModeProcessor(config["Mode"].ToString()))
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