using System;
using System.Collections.Generic;
using InfoReader.Configuration.Converter;

namespace InfoReader.Configuration.Elements
{
    public interface IConfigElement
    {
        T? GetValue<T>();
        T? GetValue<T>(IConfigConverter<T> converter);
        object? GetValue(IConfigConverter converter);
        object? GetValue(Type targetType);
        void SetValue(string key, object? val);
        IConfigElement this[string key] { get; }
        Dictionary<string, object> ToDictionary();

    }
}
