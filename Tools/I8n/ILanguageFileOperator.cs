using System.Collections.Generic;

namespace InfoReader.Tools.I8n
{
    public interface ILanguageFileOperator
    {
        Dictionary<string, string> ReadAll(string languageId);
        void Write(string languageId, string key, string val);
    }
}
