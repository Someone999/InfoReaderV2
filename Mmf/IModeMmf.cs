namespace InfoReader.Mmf
{
    public interface IModeMmf : IMmf
    {
        MmfGameMode EnabledMode { get; set; }
    }
}
