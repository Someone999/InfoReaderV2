using System;
using System.Collections.Generic;

namespace InfoReader.Mmf.Filters;

public class DefaultMmfFilter : IMmfFilter
{
    public string MmfType => "NormalMmf";
    public MmfBase Filter(Dictionary<string, object> config)
    {
        return new NormalMmf(config["Name"].ToString())
        {
            FormatFile = config["FormatFile"].ToString(),
            Enabled = bool.Parse(config["Enabled"].ToString())
        };
    }
}