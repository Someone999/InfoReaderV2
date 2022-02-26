using System.Collections.Generic;

namespace InfoReader.Configuration.Converter
{
    /*public class ConfigConverterCollection
    {
        private ConfigConverterCollection()
        {
        }

        private readonly Dictionary<Type, Dictionary<string, IConfigConverter>> _converters =
            new Dictionary<Type, Dictionary<string, IConfigConverter>>();

        private static ConfigConverterCollection? _instance;
        static readonly object StaticLocker = new object();

        public static ConfigConverterCollection GetInstance()
        {
            if (_instance != null) 
                return _instance;
            lock (StaticLocker)
            {
                _instance = new ConfigConverterCollection();
            }

            return _instance;
        }

        public bool Register(string name, IConfigConverter converter, Type genericType)
        {
            try
            {
                if (!_converters.ContainsKey(genericType))
                {
                    _converters.Add(genericType, new Dictionary<string, IConfigConverter>());
                }

                _converters[genericType].Add(name, converter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Register<T>(string name, IConfigConverter<T> converter) => Register(name, converter, typeof(T));

        public IConfigConverter GetConverter(string name, Type genericType) => _converters[genericType][name];

        public IConfigConverter<T> GetConverter<T>(string name, Type genericType) =>
            (IConfigConverter<T>) _converters[genericType][name];

        public Dictionary<string, IConfigConverter> GetConverters(Type genericType) => _converters[genericType];

        public Dictionary<string, IConfigConverter<T>> GetConverters<T>()
        {
            Dictionary<string, IConfigConverter<T>> dict = new Dictionary<string, IConfigConverter<T>>();
            foreach (var converter in _converters)
            {
                if (converter.Key != typeof(T)) 
                    continue;
                foreach (var configConverter in converter.Value)
                {
                    dict.Add(configConverter.Key, (IConfigConverter<T>) configConverter.Value);
                }
            }

            return dict;
        }
    }*/

    public interface IConfigConverter
    {
        object? Convert(object? value);
        object? ToValue(object? value);
        Dictionary<string, object>? ToDictionary(object value);
    }
    public interface IConfigConverter<T> : IConfigConverter
    {
        new T? Convert(object? value);
        Dictionary<string, object>? ToDictionary(T value);
        object? ToValue(T? value);
    }
}
