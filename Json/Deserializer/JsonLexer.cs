using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InfoReader.Exceptions;

namespace InfoReader.Json.Deserializer;

public class JsonLexer
{
    private readonly StringReader _reader;
    private readonly List<JsonToken> _tokens = new List<JsonToken>();
    public JsonToken[] Tokens => _tokens.ToArray();
    char Read()
    {
        int tmp = _reader.Read();
        if (tmp == -1)
        {
            return (char) 0;
        }

        return (char) tmp;
    }

    char Peek()
    {
        int tmp = _reader.Peek();
        if (tmp == -1)
        {
            return (char) 0;
        }

        return (char) tmp;
    }


    public JsonLexer(string input)
    {
        int quote = input.Where(c => c == '"').ToArray().Length;
        ThrowWhen(quote % 2 != 0, "Quotes count mismatched.");

        _reader = new StringReader(input);
        Parse();
    }

    void ThrowWhen(bool condition, string info)
    {
        if (condition)
        {
            throw new JsonLexerException(info);
        }
    }

    JsonToken? LastPropertyToken()
    {
        return _tokens.LastOrDefault(t => t.TokenType == JsonTokenType.PropertyName);
    }

    private void Parse()
    {
        StringBuilder val = new StringBuilder();
        Stack<JsonTokenType> startTokens = new Stack<JsonTokenType>();


        bool isNullValue = false, isTrueValue = false, isFalseValue = false;
        while (true)
        {
            JsonToken currentToken = new JsonToken(JsonTokenType.None, null,JsonValueType.None);

            char currentChar = Peek();
            if (currentChar == '\0')
            {
                break;
            }

            switch (currentChar)
            {
                case 'n':
                    ThrowWhen(isNullValue || isTrueValue || isFalseValue, "Unexpected special value.");
                    char[] tmpBuffer0 = new char[4];
                    _reader.Read(tmpBuffer0, 0, 4);
                    if (new string(tmpBuffer0) == "null")
                    {
                        isNullValue = true;
                    }

                    break;
                case 't':
                    ThrowWhen(isNullValue || isTrueValue || isFalseValue, "Unexpected special value.");
                    char[] tmpBuffer1 = new char[4];
                    _reader.Read(tmpBuffer1, 0, 4);
                    if (new string(tmpBuffer1) == "true")
                    {
                        isTrueValue = true;
                    }

                    break;
                case 'f':
                    ThrowWhen(isNullValue || isTrueValue || isFalseValue, "Unexpected special value.");
                    char[] tmpBuffer2 = new char[5];
                    _reader.Read(tmpBuffer2, 0, 5);
                    if (new string(tmpBuffer2) == "false")
                    {
                        isFalseValue = true;
                    }

                    break;

                case '"':
                {
                    ThrowWhen(isNullValue || isTrueValue || isFalseValue, "Unexpected special value.");
                    bool inArray = startTokens.Count > 0 && startTokens.Peek() == JsonTokenType.StartArray;
                    ThrowWhen(inArray && _tokens.Last().TokenType == JsonTokenType.EndObject, "The brackets are not closed.");
                    char c;
                    Read();
                    while ((c = Read()) != '"')
                    {
                        val.Append(c);
                        ThrowWhen(c == '\0', "Unexpected end.");
                    }

                    break;
                }
                case ':':
                    ThrowWhen(isNullValue || isTrueValue || isFalseValue, "Unexpected special value.");
                    currentToken.TokenType = JsonTokenType.PropertyName;
                    currentToken.TokenValue = val.ToString();
                    val.Clear();
                    _tokens.Add(currentToken);
                    _tokens.Add(new JsonToken(JsonTokenType.Colon, ":",JsonValueType.None));
                    Read();
                    break;
                case ',':
                {
                    bool notEnd = (_tokens.Last().TokenType & JsonTokenType.EndToken) == 0;
                    bool inArr = startTokens.Peek() == JsonTokenType.StartArray;
                    bool notPropVal = _tokens.Last().TokenType != JsonTokenType.PropertyValue;

                    if (notPropVal)
                    {
                        if (val.Length > 0)
                        {
                            _tokens.Add(
                                new JsonToken(JsonTokenType.PropertyValue, val.ToString(), JsonValueType.String));
                        }
                        else
                        {
                            currentToken.TokenType = JsonTokenType.PropertyValue;
                            object? obj = isNullValue ? null : isTrueValue ? true : isFalseValue ? false : val.ToString();
                            currentToken.TokenValue = obj;
                            _tokens.Add(currentToken);
                        }
                        val.Clear();
                    }
                    
                    notPropVal = _tokens.Last().TokenType != JsonTokenType.PropertyValue;
                    ThrowWhen(notPropVal && notEnd, "JSON property or end token expected.");
                    
                    
                    isNullValue = false;
                    isTrueValue = false;
                    isFalseValue = false;
                    val.Clear();
                    _tokens.Add(new JsonToken(JsonTokenType.Comma, ",",JsonValueType.None));
                    Read();
                    break;
                }
                case '{':
                    ThrowWhen(isNullValue || isTrueValue || isFalseValue, "Unexpected special value.");
                    JsonToken? lastPropertyTokenObj = LastPropertyToken();
                    if (lastPropertyTokenObj != null)
                        lastPropertyTokenObj.ValueType = JsonValueType.Object;
                    startTokens.Push(JsonTokenType.StartObject);
                    _tokens.Add(new JsonToken(JsonTokenType.StartObject, "{",JsonValueType.None));
                    Read();
                    break;
                case '}':
                {
                    ThrowWhen(startTokens.Pop() != JsonTokenType.StartObject, "StartObject token expected.");
               

                    if (_tokens.Last().TokenType == JsonTokenType.Colon)
                    {
                        _tokens.Add(new JsonToken(JsonTokenType.PropertyValue, val.ToString(),JsonValueType.String));
                        val.Clear();
                    }

                    _tokens.Add(new JsonToken(JsonTokenType.EndObject, "}",JsonValueType.None));
                    Read();
                    break;
                }
                case '[':
                    JsonToken? lastPropertyTokenArr = LastPropertyToken();
                    if (lastPropertyTokenArr != null)
                    {
                        lastPropertyTokenArr.ValueType = JsonValueType.Array;
                    }
                    ThrowWhen(isNullValue || isTrueValue || isFalseValue , "Unexpected special value.");
                    ThrowWhen(lastPropertyTokenArr == null,"JSON array must has a name");
                    startTokens.Push(JsonTokenType.StartArray);
                    _tokens.Add(new JsonToken(JsonTokenType.StartArray, "[", JsonValueType.None));
                    Read();
                    break;
                case ']':
                    ThrowWhen(startTokens.Pop() != JsonTokenType.StartArray, "End token mismatched");

                    _tokens.Add(new JsonToken(JsonTokenType.EndArray, "]",JsonValueType.None));
                    Read();
                    break;
                default:
                {
                    ThrowWhen(isNullValue || isTrueValue || isFalseValue, "Unexpected special value.");
                    if (char.IsDigit(currentChar))
                    {
                        while (char.IsDigit(currentChar))
                        {
                            if (char.IsDigit(Peek()) || Peek() == '.')
                            {
                                val.Append(currentChar = Read());
                            }
                            else
                            {
                                break;
                            }
                        }

                        var numStr = val.ToString();

                        if (numStr.Contains("."))
                        {
                            _tokens.Add(new JsonToken(JsonTokenType.PropertyValue, double.Parse(numStr),JsonValueType.Float));
                        }
                        else
                        {
                            long l = long.Parse(numStr);
                            _tokens.Add(l < int.MaxValue
                                ? new JsonToken(JsonTokenType.PropertyValue, (int) l, JsonValueType.Integer)
                                : new JsonToken(JsonTokenType.PropertyValue, l,JsonValueType.Long));
                        }
                    }
                    else
                    {
                        Read();
                    }
                    
                    //val.Clear();
                    break;
                }
            }
        }

        if (isNullValue || isTrueValue || isFalseValue || val.Length > 0)
        {
            JsonToken token = new JsonToken(JsonTokenType.PropertyValue, null, JsonValueType.None);
            object? obj = isNullValue ? null : isTrueValue ? true : isFalseValue ? false : val.ToString();
            JsonValueType valType = obj == null ? JsonValueType.Null :
                obj is bool ? JsonValueType.Bool : JsonValueType.String;
            token.TokenValue = obj;
            token.ValueType = valType;
            _tokens.Add(token);
        }

        var lastTokenType = _tokens.Last().TokenType;

        ThrowWhen(
            (startTokens.Count > 0 || (lastTokenType & JsonTokenType.EndToken) == 0) &&
            lastTokenType != JsonTokenType.PropertyValue, "Unexpected end.");


    }
}