using System;

namespace InfoReader.Json
{
    public class JsonPropertyAttribute : Attribute
    {
        public string PropertyName { get; }
        public JsonPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
