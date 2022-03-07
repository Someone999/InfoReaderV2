using System.Globalization;

namespace InfoReader.Tools.I8n
{
    
    public interface ILocalizationInfo
    {
        string CultureName { get; }
        CultureInfo Culture { get; }
        TranslationDictionary Translations { get; }
    }
}
