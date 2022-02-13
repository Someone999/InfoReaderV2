using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osuTools.OrtdpWrapper;

namespace InfoReader.Configuration.Converter
{
    public class BeatmapReadMethodsConverter: IConfigConverter<OrtdpWrapper.BeatmapReadMethods>
    {
        object? IConfigConverter.Convert(object? value)
        {
            return Convert(value);
        }

        public Dictionary<string, object> ToDictionary(OrtdpWrapper.BeatmapReadMethods value)
        {
            return new Dictionary<string, object>() {{"BeatmapReadMethod", value.ToString()}};
        }

        public OrtdpWrapper.BeatmapReadMethods Convert(object? value)
        {
            if (value is string s)
            {
                var method = (OrtdpWrapper.BeatmapReadMethods) Enum.Parse(typeof(OrtdpWrapper.BeatmapReadMethods), s);
                return method;
            }
            throw new ArgumentException();
        }

        public Dictionary<string, object> ToDictionary(object value)
        {
            return ToDictionary((OrtdpWrapper.BeatmapReadMethods) value);
        }
    }
}
