using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Configuration.Attributes
{
    [Obsolete("The behavior of property with bool type is as same as the property with this attribute.")]
    [AttributeUsage(AttributeTargets.Property)]
    public class BoolAttribute : ConfigTypeAttribute
    {
    }
}
