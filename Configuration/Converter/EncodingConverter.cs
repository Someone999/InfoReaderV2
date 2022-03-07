using System;
using System.Collections.Generic;
using System.Text;

namespace InfoReader.Configuration.Converter
{
    public class EncodingConverter : IConfigConverter<Encoding>
    {
        object? IConfigConverter.Convert(object? value)
        {
            return Convert(value);
        }

        public object? ToValue(object? value)
        {
            if (value is Encoding encoding)
            {
                return ToValue(encoding);
            }

            return null;
        }

        public Dictionary<string, object>? ToDictionary(Encoding value)
        {
            return null;
            /*return new Dictionary<string, object>
            {
                {"Encoding", value.BodyName}
            };*/
        }

        public object? ToValue(Encoding? value) => value?.BodyName;
        

        public Encoding? Convert(object? value)
        {
            try
            {
                return Encoding.GetEncoding(value?.ToString() ?? "UTF-8");
            }
            catch (Exception)
            {
                return Encoding.UTF8;
            }
        }

        public Dictionary<string, object>? ToDictionary(object value)
        {
            if (value is Encoding encoding)
                return ToDictionary(encoding);
            return null;
        }
    }
}
