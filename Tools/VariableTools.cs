using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using osuTools.Attributes;

namespace InfoReader.Tools
{
    public static class DictionaryExtension
    {
        public static void AddNotExist<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                return;
            dictionary.Add(key, value);
        }
    }
    public static class VariableTools
    {
        public static Dictionary<string, PropertyInfo> GetAvailableVariables(Type t)
        {
            Dictionary<string, PropertyInfo> availableVariables = new Dictionary<string, PropertyInfo>();
            var props = ReflectionTools.GetPropertiesWithAttribute<AvailableVariableAttribute>(t,
                BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                foreach (var availableVariable in GetAvailableVariables(prop.Item1.PropertyType))
                {
                    availableVariables.AddNotExist(availableVariable.Key,availableVariable.Value);
                }
                foreach (var attr in prop.Item2)
                {
                    availableVariables.AddNotExist(prop.Item1.Name, prop.Item1);
                    if (!attr.HasAlias)
                    {
                        continue;
                    }
                    foreach (var alias in attr.Alias)
                    {
                        availableVariables.AddNotExist(alias, prop.Item1);
                    }
                }
            }
            return availableVariables;
        }
    }
}
