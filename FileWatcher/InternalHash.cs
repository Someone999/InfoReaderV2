using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.FileWatcher
{
    public static class FilesHash
    {
        public static string RealTimePpDisplayer => "4a589d528bcabf35ce9f91dbda1129bd";
    }


    public static class Tools
    {
        public static string GetMd5String(byte[] md5Bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in md5Bytes)
            {
                sb.Append($"{b:x2}");
            }

            return sb.ToString();
        }
    }

    public class PluginFileInfo
    {
        public string Name { get;  }
        public byte[] Content { get;}
        public string Directory { get; }

        public PluginFileInfo(string filePath, byte[] content = null)
        {
            Name = Path.GetFileName(filePath);
            Directory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentException("Invalid file path.");
            }

            Content = content?.Length != 0 ? content : File.ReadAllBytes(filePath);
        }
        public bool FileChanged(string filePath)
        {
            return Tools.GetMd5String(MD5.Create().ComputeHash(File.ReadAllBytes(filePath))) !=
                   Tools.GetMd5String(MD5.Create().ComputeHash(Content));
        }

        public bool FileChanged(string filePath, string md5Hash)
        {
            return Tools.GetMd5String(MD5.Create().ComputeHash(File.ReadAllBytes(filePath))) !=
                   md5Hash;
        }

        public string BackupDirectory { get; set; }
        public bool ReplaceFile(string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(Directory, Name);
            }
            if (!File.Exists(filePath))
            {
                return false;
            }
            string dir = BackupDirectory;
            if (!string.IsNullOrEmpty(Path.GetFileName(dir)))
                dir = Path.GetDirectoryName(dir);
            try
            {
                File.Move(filePath, Path.Combine(dir ?? ".\\Backup", Name + new TimeSpan(DateTime.UtcNow.Ticks).TotalSeconds));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return File.Exists(
                Path.Combine(dir ?? ".\\Backup", Name + new TimeSpan(DateTime.UtcNow.Ticks).TotalSeconds)) && !FileChanged(filePath);
        }

        public bool ReleaseFile(string filePath = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(Directory, Name);
            }
            File.WriteAllBytes(filePath, Content);
            return File.Exists(filePath);
        }

        public bool DeleteFile()
        {
            File.Delete(Path.Combine(Directory, Name));
            return !File.Exists(Path.Combine(Directory, Name));
        }
    }
}
