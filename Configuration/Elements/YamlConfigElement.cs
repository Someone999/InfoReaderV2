using System;
using System.Collections.Generic;
using System.IO;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Serializer;
using YamlDotNet.Serialization;

namespace InfoReader.Configuration.Elements
{
    public class YamlConfigElement : IConfigElement, IConfigWriter , IConfigSerializer
    {
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
        public object? GetValue(Type targetType) => Convert.ChangeType(_innerVal, targetType);

        public void SetValue(string key, object? val)
        {
            if (_innerVal is Dictionary<object, object> dict)
            {
                dict[key] = val;
            }
        }


        public IConfigElement this[string key]
        {
            get
            {
                Dictionary<string, object> dict = _innerVal as Dictionary<string, object> ?? throw new ArgumentException();
                return new YamlConfigElement(dict[key]);
            }
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new YamlDictionaryConverter().Convert(_innerVal as Dictionary<object, object> ??
                                                         throw new InvalidOperationException(
                                                             "Type of value is not a dictionary."));
        }

        private readonly object? _innerVal;
        public YamlConfigElement(string path)
        {
            StringReader reader = new StringReader(File.ReadAllText(path));
            Deserializer yamlDeserializer = new Deserializer();
            _innerVal = yamlDeserializer.Deserialize(reader);
            _innerVal = ToDictionary();
        }
        private YamlConfigElement(object val) => _innerVal = val;
        public void WriteToFile(string path)
        {
            File.WriteAllText(path, Serialize());
        }

        public string Serialize()
        {
            return new YamlDotNet.Serialization.Serializer().Serialize(_innerVal ?? new Dictionary<object,object>());
        }
    }
}