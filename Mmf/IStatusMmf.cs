using osuTools.Game;

namespace InfoReader.Mmf
{
    public interface IStatusMmf : IMmf
    {
        OsuGameStatus EnabledStatus { get; set; }
    }
}
