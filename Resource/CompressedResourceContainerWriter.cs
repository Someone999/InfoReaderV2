using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;

namespace InfoReader.Resource;

public interface IResourceContainerWriter
{
    List<string> Files { get; set; }
    FileStream? WriteToFile(string path, bool autoClose = true);
    Stream? Write(Stream outputStream, bool autoClose = true);
}

public class CompressedResourceContainerWriter : IResourceContainerWriter
{
    public List<string> Files { get; set; } = new List<string>();
    public FileStream? WriteToFile(string path, bool autoClose = true) => (FileStream?)Write(File.Create(path), autoClose);
    public Stream? Write(Stream outputStream, bool autoClose = true)
    {
        MemoryStream contentStream = new MemoryStream();
        BinaryWriter? writer = ResourceContainerTools.GeneralWriter(Files, contentStream, false);
        if (writer == null)
        {
            throw new InvalidOperationException();
        }
        writer.Flush();
            
        contentStream.Seek(0, SeekOrigin.Begin);
        GZip.Compress(contentStream, outputStream, autoClose);
        return autoClose ?  null : outputStream;
    }
}