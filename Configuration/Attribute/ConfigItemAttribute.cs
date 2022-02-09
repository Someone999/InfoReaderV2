
using System;

namespace InfoReader.Configuration.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigItemAttribute : System.Attribute
    {
        public string? DisplayName { get; }
        public string ConfigPath { get; }
        public Type? ConverterType { get; }
        public ConfigItemAttribute(string configPath = "", string? displayName = null, Type? converterType = null)
        {
            ConfigPath = configPath;
            DisplayName = displayName;
            ConverterType = converterType;
        }
    }
}
