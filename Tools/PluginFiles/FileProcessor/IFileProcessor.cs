namespace InfoReader.Tools.PluginFiles.FileProcessor
{
    public interface IFileProcessor
    {
        bool Process(string filePath, string? target);
    }
}
