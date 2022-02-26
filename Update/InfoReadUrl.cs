using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Update
{
    internal static class InfoReadUrl
    {
        internal static string TestUrl => "https://localhost:44381";
        internal static string BaseUrl => "http://archaring.xyz";
        internal static string GetFilesUrl => $"{TestUrl}/plugin/files?version={{0}}";
        internal static string AvailabilityStatusUrl => $"{TestUrl}/plugin/availability?version={{0}}";
    }
}
