
using System.Linq;
using System.Net.Http;
using InfoReader.Json.Deserializer;
using InfoReader.Update;


namespace InfoReader.Version
{
    public class PluginVersion
    {
        /// <summary>
        /// This value will be returned when query or parse failed.
        /// </summary>
        public static PluginVersion InvalidVersion { get; } = new(0, 0, -1);
        protected bool Equals(PluginVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Addition == other.Addition;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PluginVersion) obj);
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

        public int GetVersionId()
        {
            HttpClient client = new HttpClient();
            string rslt = client.GetStringAsync(string.Format(InfoReadUrl.VersionIdQueryUrl, ToString())).Result;

            var pluginVersion = JsonDeserializer.Deserialize(rslt);

            if (pluginVersion == null)
                return -1;
            return pluginVersion["version_id"]?.GetValue<int>() ?? -1;
        }

        public static PluginVersion Parse(string version)
        {
            string[] parts = version.Split('.');
            if (parts.Length != 3)
            {
                return InvalidVersion;
            }
            int[] parsed = (from part in parts select int.Parse(part)).ToArray();
            return new PluginVersion(parsed[0], parsed[1], parsed[2]);
        }

        public static PluginVersion LatestVersion => InfoReaderUpdateTools.GetLatestVersion();
        public string? GetChangelog()
        {
            HttpClient client = new HttpClient();
            return client.GetStringAsync(string.Format(InfoReadUrl.ChangelogQueryUrl, ToString())).Result;
        }

        public static bool operator ==(PluginVersion a, PluginVersion b)
        {
            return a.Major == b.Major && a.Minor == b.Minor && a.Addition == b.Addition;
        }

        int ToConvertible()
        {
            return Major * 100 + Minor * 10 + Addition;
        }
        public static bool operator !=(PluginVersion a, PluginVersion b)
        {
            return !(a == b);
        }

        public static bool operator >(PluginVersion a, PluginVersion b)
        {
            return a.ToConvertible() > b.ToConvertible();
        }

        public static bool operator <(PluginVersion a, PluginVersion b)
        {
            return a.ToConvertible() < b.ToConvertible();
        }
        public static bool operator >=(PluginVersion a, PluginVersion b)
        {
            return a > b || a == b;
        }

        public static bool operator <=(PluginVersion a, PluginVersion b)
        {
            return a < b || a == b;
        }

        public int Major { get; }
        public int Minor { get;}
        public int Addition { get; }
        public override string ToString() => $"{Major}.{Minor}.{Addition}";

        public PluginFile[] GetPluginFiles() => InfoReaderUpdateTools.GetUpdateFiles(ToString());
        public AvailabilityStatus GetAvailabilityStatus() => InfoReaderUpdateTools.GetAvailabilityStatus(ToString());
    }
}
