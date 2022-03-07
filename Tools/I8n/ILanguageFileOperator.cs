namespace InfoReader.Tools.I8n
{
    public interface ILanguageFileOperator
    {
        TranslationDictionary ReadAll(string languageId);
        void Write(string languageId, string key, string val);
    }
}
