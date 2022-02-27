using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Version;

namespace InfoReader.Attributes
{
    public class InfoReaderVersionAttribute : Attribute
    {
        public PluginVersion? PluginVersion { get; }
        public InfoReaderVersionAttribute(string version) => PluginVersion = PluginVersion.Parse(version);
    }
}
