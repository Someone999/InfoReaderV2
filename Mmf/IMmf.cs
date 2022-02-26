using System.IO;

namespace InfoReader.Mmf;

public interface IMmf
{
    string Name { get; set; }
    System.IO.MemoryMappedFiles.MemoryMappedFile MappedFile { get; set; }
    Stream MappedFileStream { get; }
    int UpdateInterval { get; set; }
    bool Enabled { get; set; }
    string FormatFile { get; set; }
    string Format { get; }
    event EnabledStateChangedEventHandler? OnEnabledStateChanged;
    void Update(InfoReaderPlugin instance);
    string MmfType { get; }
}