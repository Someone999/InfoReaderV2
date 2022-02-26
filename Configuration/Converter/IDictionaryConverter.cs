using System.Collections.Generic;

namespace InfoReader.Configuration.Converter;

public interface IDictionaryConverter<T>
{
    string ConfigType { get; }
    Dictionary<string, object?> Convert(T originalDictionary);
}