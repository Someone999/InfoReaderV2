using System;
using System.Collections.Generic;
using osuTools.OrtdpWrapper;

namespace InfoReader.Configuration.Converter
{
    public class BeatmapReadMethodsConverter: IConfigConverter<OrtdpWrapper.BeatmapReadMethods>
    {
        object? IConfigConverter.Convert(object? value)
        {
            return Convert(value);
        }

        public object? ToValue(object? value)
        {
            if (value is OrtdpWrapper.BeatmapReadMethods method)
            {
                return ToValue(method);
            }
            return null;
        }

        public Dictionary<string, object>? ToDictionary(OrtdpWrapper.BeatmapReadMethods value)
        {
            return null; //new Dictionary<string, object>() {{"BeatmapReadMethod", value.ToString()}};
        }

        public object? ToValue(OrtdpWrapper.BeatmapReadMethods value)
        {
            return value.ToString();
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

        public Dictionary<string, object>? ToDictionary(object value)
        {
            return null; //ToDictionary((OrtdpWrapper.BeatmapReadMethods) value);
        }
    }
}
