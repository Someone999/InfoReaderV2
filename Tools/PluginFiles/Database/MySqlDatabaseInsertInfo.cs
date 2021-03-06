using InfoReader.Update;

namespace InfoReader.Tools.PluginFiles.Database;

public class MySqlDatabaseInsertInfo : DatabaseInsertInfo
{
    public override char IdentifierStart => '`';
    public override char IdentifierEnd => '`';
    public override string TableName => DatabaseInfo.TableName;
}