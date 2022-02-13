using InfoReader.Tools;
using osuTools.Game.Modes;

namespace InfoReader.Mmf
{
    public class ModeMmf: MmfBase, IModeMmf
    {
        public MmfGameMode EnabledMode { get; set; }
        public ModeMmf(string name, MmfGameMode mode) : base(name)
        {
            EnabledMode = mode;
        }

        public override void Update(InfoReaderPlugin instance)
        {
            if (instance.MemoryDataSource?.CurrentMode is not ILegacyMode legacyMode) 
                return;
            bool containsMode = legacyMode.LegacyMode.ContainsMode(EnabledMode);
            if (containsMode)
            {
                base.Update(instance);
            }
        }
    }
}
