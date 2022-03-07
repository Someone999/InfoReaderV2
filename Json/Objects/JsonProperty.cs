using System;

namespace InfoReader.Json.Objects;

public class JsonProperty : IJsonContainer
{
    internal JsonProperty(string propertyName, JsonValue propertyValue, IJsonContainer? parent)
    {
        PropertyName = propertyName;
        PropertyValue = propertyValue;
        Parent = parent;
    }

    public string Name => PropertyName;

    public IJsonContainer? this[object key]
    {
        get
        {
            if (key is not string s) 
                return null;
            return s == PropertyName ? PropertyValue : null;
        }
    }
    public string PropertyName { get; }
    public JsonValue PropertyValue { get; }
    public IJsonContainer? Parent { get; }
    public T? GetValue<T>()
    {
        return PropertyValue.GetValue<T>();
    }

    public object? GetValue(Type t)
    {
        return PropertyValue.GetValue(t);
    }

    public int Count => 1;
}