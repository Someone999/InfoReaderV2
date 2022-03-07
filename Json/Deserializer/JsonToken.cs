namespace InfoReader.Json.Deserializer;

public class JsonToken
{
    public JsonTokenType TokenType { get; internal set; }
    public object? TokenValue { get; internal set; }
    public JsonValueType ValueType { get; internal set; }

    public JsonToken(JsonTokenType tokenType, object? tokenValue, JsonValueType valueType)
    {
        TokenType = tokenType;
        TokenValue = tokenValue;
        ValueType = valueType;
    }

    public override string ToString()
    {
        return $"{TokenType} {TokenValue}";
    }
}