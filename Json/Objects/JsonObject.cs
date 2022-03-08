using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfoReader.Json.Deserializer;
using InfoReader.Tools;

namespace InfoReader.Json.Objects;



public class JsonObject : IJsonContainer
{

    List<IJsonContainer> _containers = new();

    public JsonObject(IJsonContainer? parent, string? name)
    {
        Parent = parent;
        Name = name;
    }

    public string? Name { get; }
    public IJsonContainer this[object key] => _containers.First(e => e.Name == key.ToString());
    public int Count => _containers.Count;
    public JsonValueType ValueType => JsonValueType.Object;

    public void Add(IJsonContainer jsonContainer)
    {
        _containers.Add(jsonContainer);
    }

    public IJsonContainer? Parent { get; }
    public T? GetValue<T>()
    {
        return (T?)GetValue(typeof(T));
    }

    public object? GetValue(Type t)
    {
        if (_containers.Count == 0)
            return default;
        if (t.IsAbstract || t.IsInterface)
            return default;
        object? obj = ReflectionTools.CreateInstance(t, Array.Empty<object>());
        if (obj == null)
            return default;
        MemberInfo[] members = t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        Dictionary<MemberInfo, bool> memberReadStateMap = new Dictionary<MemberInfo, bool>();
        foreach (var memberInfo in members)
        {
            memberReadStateMap.Add(memberInfo, false);
        }
        Dictionary<IJsonContainer, MemberInfo> matched = new Dictionary<IJsonContainer, MemberInfo>();
        foreach (var container in _containers)
        {
            foreach (MemberInfo member in members)
            {
                if (memberReadStateMap[member])
                {
                    //Console.WriteLine($"Scanned member {member.Name}");
                    continue;
                }
                if (member.IsDefined(typeof(JsonPropertyAttribute)))
                {
                    var attr = member.GetCustomAttribute<JsonPropertyAttribute>();
                    if (attr == null || attr.PropertyName != container.Name)
                        continue;
                    memberReadStateMap[member] = true;
                    matched.Add(container, member);
                    break;
                }

                if (member.Name != container.Name)
                    continue;
                memberReadStateMap[member] = true;
                matched.Add(container, member);
                break;
            }
        }
        


        SetTypeMembers(t, matched, obj);
        return obj;
    }

    void SetTypeMembers(Type type, Dictionary<IJsonContainer, MemberInfo> map, object obj)
    {
        foreach (var match in map)
        {
            MemberInfo member = match.Value;
            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo? prop = member as PropertyInfo;
                if (prop == null)
                {
                    continue;
                }

                if (match.Key is JsonProperty jsonProperty)
                {
                    prop.SetValue(obj, jsonProperty.GetValue<object>());
                }
                else if (match.Key is JsonArray jsonArray)
                {
                    prop.SetValue(obj, jsonArray.GetValue(prop.PropertyType));
                }
                else if(match.Key is JsonObject jsonObject)
                {
                    Type t = prop.PropertyType;
                    prop.SetValue(obj, jsonObject.GetValue(t));
                }

            }
            else if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo? field = member as FieldInfo;
                if (field == null)
                {
                    continue;
                }

                if (match.Key is JsonProperty jsonProperty)
                {
                    field.SetValue(obj, jsonProperty.GetValue<object>());
                }
                else if (match.Key is JsonArray jsonArray)
                {
                    field.SetValue(obj, jsonArray.GetValue(field.FieldType));
                }
                else if (match.Key is JsonObject jsonObject)
                {
                    Type t = field.FieldType;
                    field.SetValue(obj, jsonObject.GetValue(t));
                }
            }
        }
    }
}