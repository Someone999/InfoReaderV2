using InfoReader.Update;

namespace InfoReader.Tools.PluginFiles.Database;

public class SqlServerDatabaseInsertInfo : DatabaseInsertInfo
{
    public override char IdentifierStart => '[';
    public override char IdentifierEnd => ']';
    public override string TableName => DatabaseInfo.TableName;
}