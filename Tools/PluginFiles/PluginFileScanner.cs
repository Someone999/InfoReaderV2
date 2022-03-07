using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using InfoReader.Tools.PluginFiles.Database;
using InfoReader.Update;
using InfoReader.Version;
using osuTools.MD5Tools;

namespace InfoReader.Tools.PluginFiles
{
    public class PluginFileScanner
    {
        public string[] Files { get; private set; } = Array.Empty<string>();

        public PluginFile[] ToPluginFiles(PluginVersion version)
        {
            List<PluginFile> pluginFiles = new();
            MD5 md5 = MD5.Create();
            foreach (var file in Files)
            {
                string ext = Path.GetExtension(file).Substring(1);
                if (ext == "pdb")
                {
                    continue;
                }

                byte[] bts = File.ReadAllBytes(file);
                string md5Str = MD5String.GetString(md5.ComputeHash(bts));
                string relatedPath =
                    file.Replace(
                        Path.GetDirectoryName(file) ??
                        throw new InvalidOperationException(), "");
                if (relatedPath.StartsWith(".\\"))
                {
                    relatedPath = relatedPath.Substring(2);
                }

                
               
                if (string.IsNullOrEmpty(relatedPath))
                {
                    continue;
                }

                string serverRelatedPath = ServerTools.GetServerRelatedPath(version, relatedPath).Replace("\\", "/");
                string friendlyName = Path.GetFileNameWithoutExtension(relatedPath);
                pluginFiles.Add(new PluginFile
                {
                    FileName = serverRelatedPath,
                    FriendlyName = friendlyName,
                    DownloadPath = relatedPath,
                    Md5Hash = md5Str
                });
            }

            return pluginFiles.ToArray();
        }

        public void Scan(string path = ".", string pattern = "*")
        {
            Files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
        }

        public string ToSqlInsert(DatabaseInsertInfo insertInfo,PluginVersion version) => insertInfo.GenerateSql(ToPluginFiles(version),version);

    }
}
