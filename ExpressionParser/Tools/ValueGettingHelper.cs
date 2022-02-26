using System;
using System.Linq;
using System.Reflection;
using InfoReader.ExpressionParser.Nodes;

namespace InfoReader.ExpressionParser.Tools
{
    public static class ValueGettingHelper
    {
        public static bool TryGetPropertyValue<T>(string fullName, object? rootReflectObj, out T? val, out object? failedObject, out string? failedMember)
        {
            if (rootReflectObj == null)
            {
                val = default;
                failedMember = "#null";
                failedObject = NullNode.Null;
                return false;
            }
            val = default;
            string[] splited = fullName.Split('.');
            if (splited.Any(s => string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s)))
                throw new ArgumentException("Empty or space can not presented in full name.");
            Type t = rootReflectObj.GetType();
            PropertyInfo[]? properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            int matched = 0;
            foreach (var name in splited)
            {
                string lowerName = name.ToLower();
                foreach (var property in properties ?? Array.Empty<PropertyInfo>())
                {
                    if (property.Name.ToLower() != lowerName) 
                        continue;
                    var curProperty = property;
                    rootReflectObj = curProperty?.GetValue(rootReflectObj);
                    properties = rootReflectObj?.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
                    matched++;
                }
            }
            if (matched < splited.Length)
            {
                failedMember = splited[matched];
                failedObject = rootReflectObj;
                return false;
            }
            failedObject = null;
            val = (T?)rootReflectObj;
            failedMember = null;
            return true;
        }

        public static T? GetPropertyValue<T>(string fullName, object rootReflectObj)
        {
            if (TryGetPropertyValue(fullName, rootReflectObj, out T? val, out object? failedObj, out string? failedMember))
                return val;
            throw new MissingMemberException($"Can not find any public instance or public static property \"{failedMember}\" in the object of Type \"{failedObj?.GetType()}\"");
        }
    }
}
