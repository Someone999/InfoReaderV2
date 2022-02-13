using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using InfoReader.Mmf;
using InfoReader.Mmf.Filters;

namespace InfoReader.Configuration.Converter
{
    public class MmfListConverter : IConfigConverter<List<Mmf.MmfBase>>
    {
        protected InfoReaderPlugin Plugin;
        public MmfListConverter(InfoReaderPlugin infoReader)
        {
            Plugin = infoReader;
        }
        object? IConfigConverter.Convert(object? value)
        {
            return Convert(value);
        }

        public Dictionary<string, object> ToDictionary(List<Mmf.MmfBase>? value)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type t = typeof(Mmf.MmfBase);
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var mmf in value ?? new List<Mmf.MmfBase>())
            {
                Dictionary<string, string> cfg = new Dictionary<string, string>();
                dic.Add(mmf.Name, mmf);
            }

            return dic;
        }

        public Dictionary<string, object> ToDictionary(object value)
        {
            return ToDictionary(value as List<Mmf.MmfBase>);
        }

        public virtual List<Mmf.MmfBase>? Convert(object? value)
        {
            var dictionaries = value as Dictionary<string, object>[];
            if (dictionaries == null)
                return null;
            List<Mmf.MmfBase> mmfs = new List<Mmf.MmfBase>();
            foreach (var mmfCfg in dictionaries)
            {
                foreach (var dictionary in dictionaries)
                {
                    var name = dictionary["Name"];
                    string type = dictionary["Type"].ToString();
                    IMmfFilter filter = MmfFilters.GetFilters().GetFilter(type);
                    MmfManager.GetInstance(Plugin).Add(filter.Filter(dictionary));
                }
            }
            return MmfManager.GetInstance(Plugin).Mmfs.ToList();
        }

    }
}