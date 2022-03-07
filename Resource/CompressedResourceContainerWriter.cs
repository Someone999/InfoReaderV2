using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;

namespace InfoReader.Resource;

public class CompressedResourceContainerWriter : IResourceContainerWriter
{
    private List<Stream> _streams = new List<Stream>();
    public List<string> Files { get; set; } = new List<string>();
    public FileStream? WriteToFile(string path, bool autoClose = true) => (FileStream?)Write(File.Create(path), autoClose);
    public void Close()
    {
        int count = 0;
        foreach (var stream in _streams)
        {
            //Console.WriteLine($"[{nameof(CompressedResourceContainerWriter)}] Stream{count++} closed");
            stream.Close();
        }
    }

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
        _streams.Add(outputStream);
        return autoClose ?  null : outputStream;
    }
}