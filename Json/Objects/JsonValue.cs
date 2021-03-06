using System;
using InfoReader.Json.Deserializer;

namespace InfoReader.Json.Objects;

public class JsonValue : IJsonContainer
{
    private object? _innerValue;

    internal JsonValue(object? innerValue, IJsonContainer? parent, string name, JsonValueType valueType)
    {
        _innerValue = innerValue;
        Parent = parent;
        Name = name;
        ValueType = valueType;
    }

    public string Name { get; }
    public IJsonContainer? this[object key] => null;

    public IJsonContainer? Parent { get; internal set; }
    public T? GetValue<T>()
    {
        return (T?) GetValue(typeof(T));
    }

    public object? GetValue(Type t)
    {
        if (_innerValue == null)
        {
            throw new InvalidOperationException("Can not access a null value.");
        }
        if (t == typeof(object))
        {
            return _innerValue;
        }
        return Convert.ChangeType(_innerValue, typeof(object));
    }

    public int Count => 1;
    public JsonValueType ValueType { get; }
}