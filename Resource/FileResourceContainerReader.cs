using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InfoReader.Resource;

public class FileResourceContainerReader : IResourceContainerReader
{
    private List<ResourceFileInfo> _resources = new List<ResourceFileInfo>();
    private readonly Stream _innerStream;
    public FileResourceContainerReader(string path, bool autoClose)
    {
        if (!File.Exists(path))
        {
            _innerStream = Stream.Null;
            _innerStream.Close();
        }
        else
        {
            _innerStream = File.OpenRead(path);
            Parse(_innerStream, autoClose);
        }
    }

    public FileResourceContainerReader(Stream stream, bool autoClose)
    {
        Parse(stream, autoClose);
        _innerStream = stream;
    }
    
    void Parse(Stream stream, bool autoClose)
    {
        _resources = ResourceContainerTools.GeneralParser(stream, autoClose);
    }

    public int Count => _resources.Count;
    public bool IsCompressed => false;
    public ResourceFileInfo? GetResource(string resourceName)
    {
        return _resources.FirstOrDefault(r => r.ResourceName.Equals(resourceName,StringComparison.InvariantCultureIgnoreCase));
    }

    public ResourceFileInfo[] GetResources()
    {
        return _resources.ToArray();
    }

    public void Close()
    {
        //Console.WriteLine($"[{nameof(FileResourceContainerReader)}] Stream closed");
        _innerStream.Close();
    }
}