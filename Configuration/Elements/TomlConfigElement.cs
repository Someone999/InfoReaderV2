using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Serializer;
using Nett;

namespace InfoReader.Configuration.Elements
{
    
    public class TomlConfigElement : IConfigElement, IConfigWriter, IConfigSerializer
    {
        private readonly object? _innerVal;
        public TomlConfigElement(string path)
        {
            var d = Toml.ReadFile(path);
            _innerVal = d.ToDictionary();
        }

        private TomlConfigElement(object? innerVal) => _innerVal = innerVal;
        public T? GetValue<T>()
        {
            if (_innerVal == null)
                return default;
            if (_innerVal?.GetType() == typeof(T))
            {
                return (T)_innerVal;
            }
            return (T)Convert.ChangeType(_innerVal, typeof(T));
        }

        public T? GetValue<T>(IConfigConverter<T> converter) => converter.Convert(_innerVal);

        public object? GetValue(IConfigConverter converter) => converter.Convert(_innerVal);

        public object? GetValue(Type targetType)
        {
            if (_innerVal == null)
                return default;
            if (_innerVal is TomlObject tomlObj)
            {
                return tomlObj.Get(targetType);
            }
            return Convert.ChangeType(_innerVal, targetType);
        }

        public void SetValue(string key, object? val)
        {
            if (_innerVal is Dictionary<string,object?> dict)
            {
                dict[key] = val;
            }
        }

        public IConfigElement this[string key]
        {
            get
            {
                if (_innerVal is Dictionary<string,object> dict)
                {
                    return new TomlConfigElement(dict[key]);
                }
                throw new ArgumentException();
            }
            
        }

        public Dictionary<string, object> ToDictionary()
        {
            return _innerVal as Dictionary<string, object> ??
                   throw new InvalidOperationException("Type of value is not a dictionary.");
        }

        public void WriteToFile(string path)
        {
            File.WriteAllText(path, Serialize());
        }

        public string Serialize()
        {
            return Toml.WriteString(_innerVal);
        }
    }
}
