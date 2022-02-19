using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InfoReader.Tools
{
    public static class ReflectionTools
    {
        public static Type?[] GetParameterTypes(object?[] parameters)
        {
            return parameters.Select(parameter => parameter?.GetType()).Where(t => t != null).ToArray();
        }

        public static object? CreateInstance(Type t, object?[] arguments)
        {
            if (t.IsInterface || t.IsAbstract)
            {
                return null;
            }
            var constructor = t.GetConstructor(GetParameterTypes(arguments));
            List<ConstructorInfo> matchedConstructors = new List<ConstructorInfo>();
            if (constructor == null)
            {
                ConstructorInfo[] constructors = t.GetConstructors();
                foreach (var constructorInfo in constructors)
                {
                    int paraCount = constructorInfo.GetParameters().Length;
                    if (paraCount == arguments.Length)
                    {
                        matchedConstructors.Add(constructorInfo);
                        if (matchedConstructors.Count > 1)
                        {
                            throw new AmbiguousMatchException($"Two or more constructors have {paraCount} parameters.");
                        }
                    }
                }
                constructor = matchedConstructors[0];
            }

            return constructor?.Invoke(arguments);
        }

        public static T? CreateInstance<T>(object[] arguments) => (T?)CreateInstance(typeof(T), arguments);

        public static Type[] GetTypesWithInterface<T>(Assembly asm)
        {
            List<Type> types = new List<Type>();
            foreach (var type in asm.GetTypes())
            {
                if (type.GetInterface(typeof(T).FullName) != null)
                {
                    types.Add(type);
                }
            }
            return types.ToArray();
        }

        public static (PropertyInfo,T[])[] GetPropertiesWithAttribute<T> (Type type,BindingFlags bindingAttr) where T : Attribute
        {
            List<(PropertyInfo,T[])> propertyInfos = new();
            foreach (var propertyInfo in type.GetProperties(bindingAttr))
            {
                if (propertyInfo.IsDefined(typeof(T)))
                {
                    propertyInfos.Add((propertyInfo,propertyInfo.GetCustomAttributes<T>().ToArray()));
                }
            }
            return propertyInfos.ToArray();
        }

        public static object? GetPropertyValue(PropertyInfo property, object ins)
        {
            return property.GetValue(ins) ?? null;
        }
    }

   
}
