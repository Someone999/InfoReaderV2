using System;
using osuTools.Game;
using osuTools.Game.Modes;
using osuTools.OrtdpWrapper;

namespace InfoReader.Mmf;

public class StatusModeMmf : MmfBase
{
    public OsuGameMode EnabledMode { get; set; }
    public OsuGameStatus EnabledStatus { get; set; }
    public StatusModeMmf(string name, OsuGameMode mode, OsuGameStatus status) : base(name)
    {
        EnabledStatus = status;
        EnabledMode = mode;
    }

    public override void Update(InfoReaderPlugin instance)
    {
        OrtdpWrapper ortdp = instance.MemoryDataSource as OrtdpWrapper ?? throw new ArgumentException();
        if (ortdp.CurrentMode is not ILegacyMode legacyMode)
            return;
        if (legacyMode.LegacyMode == EnabledMode && (ortdp.CurrentStatus & EnabledStatus) != 0)
        {
            base.Update(instance);
        }
    }
}