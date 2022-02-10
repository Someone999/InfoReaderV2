using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Serializer;

namespace InfoReader.Configuration.Elements
{
    public class IniConfigElement : IConfigElement, IConfigWriter, IConfigSerializer
    {
        Dictionary<string, object?> _config = new();
        private object? _innerVal;

        public IniConfigElement(object? innerVal)
        {
            _innerVal = innerVal;
        }
        public IniConfigElement(string path)
        {
            string[] lines = File.ReadAllLines(path);
            Parse(lines);
            _innerVal = _config;
        }

        void Parse(string[] lines)
        {
            if (_innerVal != null)
            {
                return;
            }
            string appKey = string.Empty;
            foreach (string line in lines)
            {
                var nLine = line.Trim();
                int firstEqualIdx = -1;
                if (nLine.StartsWith("[") && nLine.EndsWith("]"))
                {
                    appKey = nLine.Substring(1, nLine.Length - 2);
                }
                else if ((firstEqualIdx = nLine.IndexOf('=')) != -1)
                {
                    string property = nLine.Substring(0, firstEqualIdx).Trim();
                    string value = nLine.Substring(firstEqualIdx + 1).Trim();
                    if (!_config.ContainsKey(appKey))
                    {
                        _config.Add(appKey, new Dictionary<string, object>());
                    }

                    var subDict = _config[appKey];
                    if(subDict != null)
                       ((Dictionary<string,object>)subDict).Add(property, value);
                }
                else if (nLine.StartsWith("#"))
                {
                }
            }
        }

        public T? GetValue<T>()
        {
            if (_innerVal?.GetType() == typeof(T))
            {
                return (T) _innerVal;
            }

            return (T)Convert.ChangeType(_innerVal, typeof(T));
        }

        public T? GetValue<T>(IConfigConverter<T> converter) => converter.Convert(_innerVal);
        

        public object? GetValue(IConfigConverter converter) => converter.Convert(_innerVal);
        

        public object? GetValue(Type targetType)
        {
            return Convert.ChangeType(_innerVal, targetType);
        }

        public void SetValue(string key, object? val)
        {
            if (_innerVal is Dictionary<string, object?> d1)
            {
                d1[key] = val;
            }
        }
        public IConfigElement this[string key]
        {
            get
            {
                var dict = _innerVal as Dictionary<string, string> ?? throw new ArgumentException(); ;
                return new IniConfigElement(dict[key]);
            }
        }

        public Dictionary<string, object?> ToDictionary()
        {
            return _innerVal as Dictionary<string, object?> ??
                   throw new InvalidOperationException("Type of value is not a dictionary.");
        }

        public void WriteToFile(string path)
        {
            File.WriteAllText(path, Serialize());
        }

        string Extend(Dictionary<string, object>? config)
        {
            if (config == null)
                return string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (var pair in config)
            {
                builder.AppendLine($"{pair.Key} = {pair.Value}");
            }
            return builder.ToString();
        }
        public string Serialize()
        {
            StringBuilder cfgStr = new StringBuilder();
            foreach (var cfg in _config)
            {
                cfgStr.AppendLine($"[{cfg.Key}]");
                cfgStr.AppendLine(Extend(cfg.Value as Dictionary<string, object>));
            }
            return cfgStr.ToString();
        }
    }
}
