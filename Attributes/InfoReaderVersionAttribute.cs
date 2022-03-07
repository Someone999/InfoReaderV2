using System;
using InfoReader.Version;

namespace InfoReader.Attributes
{
    public class InfoReaderVersionAttribute : Attribute
    {
        public PluginVersion? PluginVersion { get; }
        public InfoReaderVersionAttribute(string version) => PluginVersion = PluginVersion.Parse(version);
    }
}
