using System;
using System.Collections.Generic;
using System.Linq;

namespace InfoReader.Configuration.Converter;

public class YamlDictionaryConverter : IDictionaryConverter<Dictionary<object, object>>
{
    public string ConfigType => "Yaml";
    Dictionary<string, object>[]? ListExtender(List<object> lst)
    {
        if (lst.FirstOrDefault() is Dictionary<object, object>)
        {
            List<Dictionary<string, object>> l = new List<Dictionary<string, object>>();
            foreach (var obj in lst)
            {
                if (obj is not Dictionary<object, object> dictionary)
                    continue;
                Dictionary<string, object> convertedDictionary = Convert(dictionary);
                l.Add(convertedDictionary);
            }
            return l.ToArray();
        }

        return null;
    }
    public Dictionary<string, object> Convert(Dictionary<object, object> originalDictionary)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        
        foreach (var pair in originalDictionary)
        {
            var val = pair.Value;

            if (val is Dictionary<object, object> d)
            {
                val = Convert(d);
            }

            if (val is List<object> l)
            {
                val = ListExtender(l);
            }

            if (!dict.ContainsKey(pair.Key.ToString()))
            {
                dict.Add(pair.Key.ToString(),val);
            }
            else
            {
                dict[pair.Key.ToString()] = val;
            }
        }
        return dict;
    }
}