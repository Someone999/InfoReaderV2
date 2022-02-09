using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

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

        public Dictionary<string, string> ReadAll(string languageId)
        {
            SQLiteConnection connection = CreateAndOpenConnection("DataSource=InfoReader.db");
            var command = connection.CreateCommand();
            command.CommandText = $"select * from '{languageId}'";
            var reader = command.ExecuteReader();
            Dictionary<string, string> result = new Dictionary<string, string>();
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