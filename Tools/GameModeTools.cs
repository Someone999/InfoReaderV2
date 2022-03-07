using InfoReader.Mmf;
using osuTools.Game.Modes;

namespace InfoReader.Tools
{
    public static class GameModeTools
    {
        public static MmfGameMode ToMmfGameMode(this OsuGameMode mode) => (MmfGameMode) (1 << (int) mode);

        public static bool ContainsMode(this MmfGameMode mmfMode, OsuGameMode mode)
        {
            return (mode.ToMmfGameMode() & mmfMode) != 0;
        }

        public static bool ContainsMode(this OsuGameMode mode, MmfGameMode mmfMode)
        {
            return (mode.ToMmfGameMode() & mmfMode) != 0;
        }

        public static bool EqualsToMode(this OsuGameMode mode, MmfGameMode mmfMode)
        {
            return mode.ToMmfGameMode() == mmfMode;
        }
        public static bool EqualsToMode(this MmfGameMode mmfMode, OsuGameMode mode)
        {
            return mode.ToMmfGameMode() == mmfMode;
        }


    }
}
