using System;
using System.IO;

namespace InfoReader.Tools.PluginFiles.FileProcessor;

public class NormalFileProcessor : IFileProcessor
{
    public bool Process(string filePath, string? target)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }
        try
        {
            if (target == null)
            {
                return true;
            }
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            string? downloadDirectory = Path.GetDirectoryName(target);
            if (downloadDirectory != null && !Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }
            File.Move(filePath, target);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}