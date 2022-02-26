using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Update;

namespace InfoReader.Version
{
    public class PluginVersion
    {
        protected bool Equals(PluginVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Addition == other.Addition;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PluginVersion) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Addition;
                return hashCode;
            }
        }

        public PluginVersion(int major, int minor, int addition)
        {
            Major = major;
            Minor = minor;
            Addition = addition;
        }

        public static PluginVersion? Parse(string version)
        {
            string[] parts = version.Split('.');
            if (parts.Length != 3)
            {
                return null;
            }
            int[] parsed = (from part in parts select int.Parse(part)).ToArray();
            return new PluginVersion(parsed[0], parsed[1], parsed[2]);
        }

        public static bool operator ==(PluginVersion a, PluginVersion b)
        {
            return a.Major == b.Major && a.Minor == b.Minor && a.Addition == b.Addition;
        }

        public static bool operator !=(PluginVersion a, PluginVersion b)
        {
            return !(a == b);
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Addition { get; set; }
        public override string ToString() => $"{Major}.{Minor}.{Addition}";

        public PluginFile[] GetPluginFiles() => InfoReaderUpdateTools.GetUpdateFiles(ToString());
        public AvailabilityStatus GetAvailabilityStatus() => InfoReaderUpdateTools.GetAvailabilityStatus(ToString());
    }
}
