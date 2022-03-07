using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InfoReader.Resource;

public class FileResourceContainerReader : IResourceContainerReader
{
    private List<Resource> _resources = new List<Resource>();
    private readonly Stream _innerStream;
    public FileResourceContainerReader(string path)
    {
        if (!File.Exists(path))
        {
            _innerStream = Stream.Null;
            _innerStream.Close();
        }
        else
        {
            _innerStream = File.OpenRead(path);
            Parse(_innerStream);
        }
    }

    public FileResourceContainerReader(Stream stream, Stream innerStream)
    {
        Parse(stream);
        _innerStream = stream;
    }
    
    void Parse(Stream stream)
    {
        _resources = ResourceContainerTools.GeneralParser(stream);
    }

    public int Count => _resources.Count;
    public bool IsCompressed => false;
    public Resource? GetResource(string resourceName)
    {
        return _resources.FirstOrDefault(r => r.ResourceName.Equals(resourceName,StringComparison.InvariantCultureIgnoreCase));
    }

    public Resource[] GetResources()
    {
        return _resources.ToArray();
    }

    public void Close()
    {
        _innerStream.Close();
    }
}