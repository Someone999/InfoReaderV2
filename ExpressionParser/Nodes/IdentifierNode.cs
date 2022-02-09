using System;
using System.Collections.Generic;
using InfoReader.ExpressionParser.Tools;

namespace InfoReader.ExpressionParser.Nodes
{
    public sealed class IdentifierNode : ValueExpressionNode
    {
        //static bool _notified = false;
        public IdentifierNode(object objNode) : base(objNode)
        {
            if (objNode is string s)
            {
                Value = s;
            }

        }

        public override ExpressionNodeType NodeType => ExpressionNodeType.Identifier;
        bool IsNumber(object? obj) => obj is not null && double.TryParse(obj.ToString(), out _);
        bool IsBoolean(object? obj) => obj is not null && bool.TryParse(obj.ToString(), out _);

        public ValueExpressionNode GetValue(object? rootReflectObject)
        {

            if (Value == null || Value.Equals(NullNode.Null) || Value.ToString() == "null")
            {
                return NullNode.Null;
            }
            if (IsNumber(Value))
                return new NumberExpressionNode(Value);
            if (IsBoolean(Value))
                return new BoolExpressionNode(Value);
            ValueGettingHelper.TryGetPropertyValue<object>(Value.ToString() ?? throw new ArgumentNullException(),
                rootReflectObject, out var obj, out _, out _);
            
            if (obj is null)
            {
                if (rootReflectObject is Dictionary<string, object> dic)
                {
                    obj = GetValueFromDictionary(dic);
                    if((ExpressionNode)obj == NullNode.Null)
                    {
                        throw new ArgumentException();
                    }
                }
            }

            if (obj is ValueExpressionNode valNode)
            {
                return valNode;
            }
            if (IsNumber(obj))
            {
                return new NumberExpressionNode(obj);
            }

            return IsBoolean(obj) ? new BoolExpressionNode(obj) : new StringExpressionNode(obj);
        }

        public ValueExpressionNode GetValueFromDictionary(Dictionary<string,object>? dict)
        {
            string? key = (string?) Value;
            if (dict is null || !dict.ContainsKey(key ?? throw new InvalidOperationException()))
            {
                return NullNode.Null;
            }
            var obj = dict[key];
            if (obj is null)
                throw new ArgumentNullException();
            if (IsNumber(obj))
                return new NumberExpressionNode(obj);
            if (IsBoolean(obj))
                return new BoolExpressionNode(obj);
            return new StringExpressionNode(obj);
            
        }
    }
}
