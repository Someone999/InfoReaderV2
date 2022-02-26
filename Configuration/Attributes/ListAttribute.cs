using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
