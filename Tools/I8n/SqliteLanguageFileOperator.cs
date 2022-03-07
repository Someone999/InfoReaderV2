using System.Data.SQLite;

namespace InfoReader.Tools.I8n
{
    public class SqliteLanguageFileOperator : ILanguageFileOperator
    {
        public SQLiteConnection CreateAndOpenConnection(string connectionString)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }

        public TranslationDictionary ReadAll(string languageId)
        {
            SQLiteConnection connection = CreateAndOpenConnection("DataSource=InfoReader.db");
            var command = connection.CreateCommand();
            command.CommandText = $"select * from '{languageId}'";
            var reader = command.ExecuteReader();
            TranslationDictionary result = new TranslationDictionary();
            while (reader.Read())
            {
                result.Add(reader["Name"].ToString(),reader["Content"].ToString());
            }
            reader.Close();
            connection.Close();
            return result;
        }

        public void Write(string languageId, string key, string val)
        {
            SQLiteConnection connection = CreateAndOpenConnection("DataSource=InfoReader.db");
            var command = connection.CreateCommand();
            command.CommandText = $"update \"{languageId}\" set ? = ? where \"Name\"=@val";
            command.Parameters.Add("val");
            command.Parameters["val"].Value = val;
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}