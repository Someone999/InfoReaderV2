using System;
using System.Collections.Generic;
using System.Reflection;
using InfoReader.Tools;

namespace InfoReader.Mmf.Filters
{
    internal class MmfFilters
    {
        private static MmfFilters? _filters;
        private readonly Dictionary<string, IMmfFilter> _filtersList = new();
        private static readonly object StaticLocker = new object();
        private void InitFilters()
        {
            Type[] types = ReflectionTools.GetTypesWithInterface<IMmfFilter>(Assembly.GetExecutingAssembly());
            foreach (var type in types)
            {
                IMmfFilter? filter = ReflectionTools.CreateInstance(type, Array.Empty<object>()) as IMmfFilter;
                if (filter == null)
                    continue;
                _filtersList.Add(filter.MmfType, filter);
            }
        }
        private MmfFilters()
        {
            InitFilters();
        }

        public static MmfFilters GetFilters()
        {
            if (_filters == null)
            {
                lock (StaticLocker)
                {
                    _filters = new MmfFilters();
                }
            }
            return _filters;
        }

        public IMmfFilter GetFilter(string filterType)
        {
            return _filtersList[filterType];
        }
    }
}
