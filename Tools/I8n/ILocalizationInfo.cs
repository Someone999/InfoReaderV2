using System.Collections.Generic;
using System.Globalization;

namespace InfoReader.Tools.I8n
{
    
    public interface ILocalizationInfo
    {
        string CultureName { get; }
        CultureInfo Culture { get; }
        Dictionary<string,string> Translations { get; }
    }
}
