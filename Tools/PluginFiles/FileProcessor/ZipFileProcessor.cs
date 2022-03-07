using System;
using System.IO;
using System.IO.Compression;

namespace InfoReader.Tools.PluginFiles.FileProcessor;

public class ZipFileProcessor : IFileProcessor
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
                return false;
            }
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            var archive = ZipFile.Open(filePath, ZipArchiveMode.Read);
            foreach (var entry in archive.Entries)
            {
                if (File.Exists(Path.Combine(target, entry.FullName)))
                {
                    File.Delete(Path.Combine(target, entry.FullName));
                }

                if (string.IsNullOrEmpty(Path.GetFileName(entry.FullName)))
                {
                    Directory.CreateDirectory(Path.Combine(target, entry.FullName));
                }
                else
                {
                    FileTools.ConfirmDirectory(Path.Combine(target, entry.FullName));
                    entry.ExtractToFile(Path.Combine(target, entry.FullName));
                }
                
            }
            //File.Delete(filePath);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}