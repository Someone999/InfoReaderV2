using System;
using System.Collections.Generic;
using InfoReader.Configuration.Elements;

namespace InfoReader.Configuration
{
    public interface IConfigurable
    {
        Type ConfigElementType { get; }
        string ConfigFilePath { get; }
        string ConfigArgName { get; }
        void Save(IConfigElement element, Dictionary<Type,object?[]>? typeConverterArgs);
    }
}
