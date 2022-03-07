using System;
using System.Collections.Generic;
using InfoReader.Configuration.Elements;

namespace InfoReader.Configuration
{
    public interface IConfigurable
    {
        Type ConfigElementType { get; }
        string ConfigFilePath { get; }
        string ConfigName { get; }
        IConfigElement? ConfigElement { get; set; }
        void Save(Dictionary<Type,object?[]>? typeConverterArgs = null);
    }
}
