using System;

namespace InfoReader.Json.Deserializer;

[Flags]
public enum JsonTokenType
{
    None = -1,
    StartObject = 1,
    EndObject = 2,
    StartArray = 4,
    EndArray = 8,
    PropertyName = 16,
    PropertyValue = 32,
    Comma = 64,
    Colon = 128,
    StartToken = StartArray | StartObject,
    EndToken = EndArray | EndObject,
}