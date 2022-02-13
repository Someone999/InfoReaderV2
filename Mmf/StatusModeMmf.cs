using System;
using osuTools.Game;
using osuTools.Game.Modes;
using osuTools.OrtdpWrapper;

namespace InfoReader.Mmf;

public class StatusModeMmf : MmfBase, IStatusMmf, IModeMmf
{
    public MmfGameMode EnabledMode { get; set; }
    public OsuGameStatus EnabledStatus { get; set; }
    public StatusModeMmf(string name, MmfGameMode mode, OsuGameStatus status) : base(name)
    {
        EnabledStatus = status;
        EnabledMode = mode;
    }

    public override void Update(InfoReaderPlugin instance)
    {
        OrtdpWrapper ortdp = instance.MemoryDataSource as OrtdpWrapper ?? throw new ArgumentException();
        if (ortdp.CurrentMode is not ILegacyMode legacyMode)
            return;
        int legacy = (int)legacyMode.LegacyMode;
        if (((1 << legacy) & (int)EnabledMode) != 0 && (ortdp.CurrentStatus & EnabledStatus) != 0)
        {
            base.Update(instance);
        }
    }
}