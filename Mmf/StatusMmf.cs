using System;
using osuTools.Game;
using osuTools.OrtdpWrapper;

namespace InfoReader.Mmf;

public class StatusMmf: MmfBase, IStatusMmf
{
    public override string MmfType => "StatusMmf";
    public OsuGameStatus EnabledStatus { get; set; }
    public StatusMmf(string name, OsuGameStatus status) : base(name)
    {
        EnabledStatus = status;
    }

    public override void Update(InfoReaderPlugin instance)
    {
        OrtdpWrapper ortdp = instance.MemoryDataSource as OrtdpWrapper ?? throw new ArgumentException();
        if ((ortdp.CurrentStatus & EnabledStatus) != 0)
        {
            base.Update(instance);
        }
    }

    
}