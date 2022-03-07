using System;
using System.Linq;

namespace InfoReader.Configuration.Attributes
{
    public class ListAttribute : ConfigTypeAttribute
    {
        public ListAttribute(string defaultSelection, params string[] items)
        {
            
            Items = items;
            if (!items.Contains(defaultSelection))
            {
                throw new InvalidOperationException();
            }
            DefaultSelection = defaultSelection;
        }
        public string DefaultSelection { get; }
        public string[] Items { get; }
    }
}
