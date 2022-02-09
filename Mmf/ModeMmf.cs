using osuTools.Game.Modes;

namespace InfoReader.Mmf
{
    public class ModeMmf: MmfBase
    {
        public OsuGameMode EnabledMode { get; set; }
        public ModeMmf(string name,OsuGameMode mode) : base(name)
        {
            EnabledMode = mode;
        }

        public override void Update(InfoReaderPlugin instance)
        {
            if (instance.MemoryDataSource?.CurrentMode is not ILegacyMode legacyMode) 
                return;
            if (legacyMode.LegacyMode == EnabledMode)
            {
                base.Update(instance);
            }
        }
    }
}
