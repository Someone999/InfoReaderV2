using System.Text;
using InfoReader.Update;
using InfoReader.Version;

namespace InfoReader.Tools.PluginFiles.Database
{
    public abstract class DatabaseInsertInfo
    {
        public abstract char IdentifierStart { get; }
        public abstract char IdentifierEnd { get; }
        public abstract string TableName { get; }

        public virtual string GenerateSql(PluginFile[] files, PluginVersion version)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var pluginFile in files)
            {
                string fileName = pluginFile.FileName;
                string friendlyName = pluginFile.FriendlyName;
                string downloadPath = pluginFile.DownloadPath;
                string md5Hash = pluginFile.Md5Hash;
                string ver = version.GetVersionId().ToString();
                string columns = DatabaseInfo.GetColumns(IdentifierStart, IdentifierEnd);
                string insertSql =
                    $"INSERT INTO {IdentifierStart}{TableName}{IdentifierEnd} {columns} values({ver},'{friendlyName}','{fileName}','{downloadPath}','{md5Hash}');";
                builder.AppendLine(insertSql);
            }

            return builder.ToString();

        }
    }
}
