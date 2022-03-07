using System.Collections.Generic;
using System.Linq;
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

        public object? ToValue(object? value)
        {
            return null;
        }

        public Dictionary<string, object> ToDictionary(List<Mmf.MmfBase>? value)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (var mmf in value ?? new List<Mmf.MmfBase>())
            {
                dic.Add(mmf.Name, mmf);
            }

            return dic;
        }

        public object? ToValue(List<MmfBase>? value)
        {
            return null;
        }

        public Dictionary<string, object> ToDictionary(object value)
        {
            return ToDictionary(value as List<Mmf.MmfBase>);
        }

        public virtual List<Mmf.MmfBase>? Convert(object? value)
        {
            dynamic? dictionaries = value as Dictionary<string, object>[];
            dictionaries ??= value as Dictionary<string, object>;
            if (dictionaries == null)
                return null;
            foreach (var mmfCfg in dictionaries)
            {

                foreach (var dictionary in dictionaries)
                {
                    dynamic tmpDict = dictionary;
                    if (dictionary is KeyValuePair<string, object> kvPair)
                    {
                        tmpDict = kvPair.Value;
                    }
                    string type = tmpDict["MmfType"].ToString();
                    IMmfFilter filter = MmfFilters.GetFilters().GetFilter(type);
                    MmfManager.GetInstance(Plugin).Add(filter.Filter(tmpDict));
                }
            }
            return MmfManager.GetInstance(Plugin).Mmfs.ToList();
        }

    }
}