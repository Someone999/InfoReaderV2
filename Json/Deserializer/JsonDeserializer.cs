using System;
using System.Collections.Generic;
using InfoReader.Json.Objects;

namespace InfoReader.Json.Deserializer
{
    public static class JsonDeserializer
    {
        public static JsonObject? Deserialize(string json)
        {
            JsonLexer lexer = new JsonLexer(json);
            if (lexer.Tokens.Length == 0)
            {
                return null;
            }

            bool inProperty = false;
            Stack<IJsonContainer> stack = new Stack<IJsonContainer>();
            
            JsonArray? currentArray;
            JsonObject? currentObject = new(null,"Root");
            var root = currentObject;
            if (lexer.Tokens[0].TokenType != JsonTokenType.StartObject)
            {
                stack.Push(currentObject);
            }

            string? propertyName = "";
            foreach (var token in lexer.Tokens)
            {
                switch (token.TokenType)
                {
                    case JsonTokenType.StartArray:
                        
                        
                        var inArray = stack.Peek() is JsonArray;
                        var inObject = stack.Peek() is JsonObject;

                        if (!inObject && !inArray)
                        {
                            throw new InvalidOperationException("Array must be in an object or in an array.");
                        }

                        currentArray = new JsonArray(stack.Peek(),
                            propertyName ?? throw new InvalidOperationException());

                        if (stack.Count > 0)
                        {
                            switch (stack.Peek())
                            {
                                case JsonObject jsonObj:
                                    jsonObj.Add(currentArray);
                                    break;
                                case JsonArray jsonArr:
                                    jsonArr.Add(currentArray);
                                    break;
                            }
                        }
                        stack.Push(currentArray);
                        break;
                    case JsonTokenType.StartObject:
                        IJsonContainer? parent = null;
                        if (stack.Count > 0)
                        {
                            parent = stack.Peek();
                        }
                        
                        currentObject = new JsonObject(parent,propertyName);
                        if (parent == null)
                        {
                            root = currentObject;
                        }

                        if (stack.Count > 0)
                        {
                            switch (stack.Peek())
                            {
                                case JsonObject jsonObj:
                                    jsonObj.Add(currentObject);
                                    break;
                                case JsonArray jsonArr:
                                    jsonArr.Add(currentObject);
                                    break;
                            }
                        }

                        stack.Push(currentObject);
                        break;
                    case JsonTokenType.PropertyName:
                        propertyName = token.TokenValue?.ToString();
                        if (propertyName == null)
                        {
                            throw new FormatException("Property name can not be null.");
                        }
                        break;
                    case JsonTokenType.Colon:
                        inProperty = true;
                        break;
                    case JsonTokenType.PropertyValue:
                        var propertyValue = token.ValueType == JsonValueType.Null ? null : token.TokenValue;
                        if (inProperty && propertyName != null)
                        {
                            if (stack.Peek() is JsonObject jsonObj)
                            {
                                jsonObj.Add(new JsonProperty(propertyName, new JsonValue(propertyValue, currentObject, propertyName), currentObject));
                            }
                            else
                            {
                                throw new InvalidOperationException("Json property can only appears in an object");
                            }
                            propertyName = "";
                        }

                        inProperty = false;
                        break;
                    case JsonTokenType.EndObject:
                       /* if (inArray)
                        {
                            currentArray.Add(currentObject);
                        }*/

                       stack.Pop();
                        break;
                    case JsonTokenType.EndArray:
                        stack.Pop();
                        break;
                    case JsonTokenType.Comma:
                        break;

                }
            }
            return root;
        }

        public static T? Deserialize<T>(string json)
        {
            var deserialize = Deserialize(json);
            if (deserialize == null)
            {
                throw new FormatException("Not a valid json text.");
            }
            return deserialize.GetValue<T>();
        }
    }
}
