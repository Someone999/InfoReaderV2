using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfoReader.Configuration.Attribute;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Mmf;
using InfoReader.Tools;
using Nett;

namespace InfoReader.Configuration
{
    public class MmfConfiguration: IConfigurable
    {
        [ConfigItem("MmfConfigs.Mmfs",converterType: typeof(MmfsConverter))]
        public List<MmfBase> MmfList { get; set; } = new();
        [ConfigItem("MmfConfigs.Encoding")]
        public string MmfEncoding { get; set; } = "UTF-8";
        public void Save(IConfigElement element, Dictionary<Type,object?[]>? typeConverterArgs)
        {
            Type cfgType = typeof(MmfConfiguration);
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public;
            foreach (var propertyInfo in ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>(cfgType, bindingAttr))
            {
                IConfigElement tmp = element;
                var cfgInfo = propertyInfo.Item2[0];
                string[] parts = cfgInfo.ConfigPath.Split('.');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    tmp = tmp[parts[i]];
                }

                object? currentValue = propertyInfo.Item1.GetValue(this);
                if (cfgInfo.ConverterType != null)
                {
                    object?[] args = Array.Empty<object>();
                    if (typeConverterArgs != null && typeConverterArgs.ContainsKey(cfgInfo.ConverterType))
                    {
                        args = typeConverterArgs[cfgInfo.ConverterType];
                    }
                    currentValue = (IConfigConverter?) ReflectionTools.CreateInstance(cfgInfo.ConverterType, args);
                }

                tmp.SetValue(parts.Last(), currentValue);
            }
        }
    }
}
