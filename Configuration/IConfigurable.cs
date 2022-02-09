using System;
using System.Collections.Generic;
using InfoReader.Configuration.Elements;

namespace InfoReader.Configuration
{
    public interface IConfigurable
    {
        void Save(IConfigElement element, Dictionary<Type,object?[]>? typeConverterArgs);
    }
}
