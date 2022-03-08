using System;
using InfoReader.Json.Deserializer;

namespace InfoReader.Json.Objects
{
    public interface IJsonContainer
    {
        string? Name { get; }
        IJsonContainer? this[object key] { get; }
        IJsonContainer? Parent { get; }
        T? GetValue<T>();
        object? GetValue(Type t);
        int Count { get; }
        JsonValueType ValueType { get; }
    }
}
