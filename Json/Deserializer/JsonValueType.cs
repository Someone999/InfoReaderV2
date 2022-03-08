using System;

namespace InfoReader.Json.Deserializer
{
    [Flags]
    public enum JsonValueType
    {
        None = 0,
        String = 1,
        Integer = 2,
        Long = 4,
        Float = 8,
        Bool = 16,
        Null = 32,
        Object = 64,
        Array = 128,
        Property = 256,
        Number = Integer | Float,
        
    }
}
