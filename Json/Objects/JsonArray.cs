using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfoReader.Json.Deserializer;
using InfoReader.Tools;

namespace InfoReader.Json.Objects
{
    public class JsonArray:IJsonContainer, IEnumerable<IJsonContainer>
    {
        private List<IJsonContainer> _containers = new();

        public JsonArray(IJsonContainer? parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        public string Name { get; }

        public IJsonContainer? this[object key]
        {
            get
            {
                if (key is int idx)
                {
                    return _containers[idx];
                }

                throw new InvalidOperationException("JsonArray need a integer index.");
            }
        }

        public void Add(JsonObject jsonObj) => _containers.Add(jsonObj);
        public void Add(JsonArray jsonObj) => _containers.Add(jsonObj);

        public IJsonContainer? Parent { get; }
        public T? GetValue<T>()
        {
            return (T?) GetValue(typeof(T));
        }

        public object? GetValue(Type t)
        {
            if (!ReflectionTools.HasInterface<IEnumerable>(t))
            {
                return null;
            }

            string? eleType = null;
            if (t.Name.EndsWith("[]"))
            {
                eleType = t.FullName?.Replace("[]", "");
            }
            MethodInfo? addMethod = t.GetMethod("Add",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static, null, new[] { t }, null);
            Type? genericArg = null;
            List<object?>? tmpList;
            

            var isGenericType = t.IsGenericType;
            if (isGenericType)
            {
                var genericArgs = t.GetGenericArguments();
                if (genericArgs.Length != 1)
                    throw new InvalidOperationException("Can not create the instance of a generic type with no or more than 1 argument.");
                genericArg = genericArgs[0];
            }

            if (eleType != null)
            {
                genericArg ??= typeof(InfoReaderPlugin).Assembly.GetType(eleType);
                
            }
            tmpList = new List<object?>();
            dynamic obj = tmpList;
            foreach (var jsonContainer in _containers)
            {
                
                object? arg = jsonContainer.GetValue(genericArg ?? typeof(object));
                if (genericArg != null)
                {
                    arg = Convert.ChangeType(arg, genericArg);
                }

                tmpList.Add(arg);
            }
            Type extraMethodClassType = typeof(Enumerable);

            var castMethod = extraMethodClassType.GetMethod("Cast", BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(IEnumerable<>) }, null);
            if (castMethod == null)
                return obj;
            var genericMethod = castMethod.MakeGenericMethod(genericArg);
            object rslt = genericMethod.Invoke(null, new object[] { tmpList });
            
            if (t.IsArray)
            {
                var toArrayMethod = extraMethodClassType.GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);
                if (toArrayMethod == null)
                    return obj;
                obj = toArrayMethod.MakeGenericMethod(genericArg).Invoke(null, new[] {rslt});
            }
            else
            {
                var toListMethod = extraMethodClassType.GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);
                if (toListMethod == null)
                    return obj;
                obj = toListMethod.MakeGenericMethod(genericArg).Invoke(null, new[] { rslt });
            }
            return obj;
        }

        public int Count => _containers.Count;
        public JsonValueType ValueType => JsonValueType.Array;

        public IEnumerator<IJsonContainer> GetEnumerator()
        {
            return _containers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
