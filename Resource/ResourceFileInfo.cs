using System.IO;
using InfoReader.Tools;

namespace InfoReader.Resource;

public class ResourceFileInfo
{
    public ResourceFileInfo(string resourcePath, long resourceSize, ResourceStream resourceStream)
    {
        ResourceName = Path.GetFileName(resourcePath);
        ResourceSize = resourceSize;
        ResourceStream = resourceStream;
        ResourcePath = resourcePath;
    }

    public string ResourceName { get; }
    public string ResourcePath { get; }
    public long ResourceSize { get; }
    public bool ForcedReplace { get;  set; }
    public ResourceStream ResourceStream { get; }

    public Stream? Write(Stream stream, bool autoClose = true)
    {
        var rs = ResourceStream;
        byte[] data = new byte[16384];
        int count = 0;
        while ((count = rs.Read(data, 0, 16384)) != 0)
        {
            //data = new byte[count];
            stream.Write(data, 0, count);
            data = new byte[count];
        }

        if (autoClose)
        {
            stream.Close();
        }
        return autoClose ? null : stream;
    }

    public FileStream? WriteToFile(string path, bool autoClose = true)
    {
        FileTools.ConfirmDirectory(path);
        var fs = File.Create(path);
        return (FileStream?) Write(fs, autoClose);
    }

    public bool Exists() => File.Exists(ResourcePath);
}