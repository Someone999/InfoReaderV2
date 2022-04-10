using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using InfoReader.Json.Deserializer;

namespace InfoReader.Resource;

public class ResourceWriteFile
{
    internal ResourceWriteFile()
    {
    }
    
    public ResourceWriteFile(string filePath, bool forcedReplace = false)
    {
        FilePath = filePath;
        ForcedReplace = forcedReplace;
    }
    public string FilePath { get; set; } = "";
    public bool ForcedReplace { get; set; }
    public string Md5 { get; set; } = "";
    public string FileName => Path.GetFileName(FilePath);
    public bool Exists => File.Exists(FilePath);
    public long GetSize()
    {
        if (File.Exists(FilePath))
        {
            return new FileInfo(FilePath).Length;
        }
        return -1;
    }

    public string GetFullFileName()
    {
        return Path.GetFullPath(FilePath);
    }

    public static ResourceWriteFile[] GetFilesFromTextFile(string filePath)
    {
        var lst = JsonDeserializer.Deserialize(File.ReadAllText(filePath))["files"].GetValue<List<ResourceWriteFile>>();
        return lst is {Count: > 0}
            ? lst.ToArray()
            : Array.Empty<ResourceWriteFile>();
    }

    public byte[] ReadAllBytes() => File.ReadAllBytes(FilePath);
}