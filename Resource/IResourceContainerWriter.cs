using System.Collections.Generic;
using System.IO;

namespace InfoReader.Resource;

public interface IResourceContainerWriter
{
    List<ResourceWriteFile> Files { get; set; }
    FileStream? WriteToFile(string path, bool autoClose = true);
    void Close();
    Stream? Write(Stream outputStream, bool autoClose = true);
}