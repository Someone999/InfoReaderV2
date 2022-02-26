
using System;
using InfoReader.Tools.I8n;

namespace InfoReader.Configuration.Attributes
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
            ConverterType = converterType;
            if (displayName == null)
                return;
            if (displayName.StartsWith("L::"))
            {
                var translations = LocalizationInfo.Current.Translations;
                var key = displayName.Substring(3);
                DisplayName = translations.ContainsKey(key) ? LocalizationInfo.Current.Translations[key] : "null";
            }
            else
            {
                DisplayName = displayName;
            }
        }
    }
}
