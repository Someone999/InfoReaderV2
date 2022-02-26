using InfoReader.ExpressionParser.Tools;

namespace InfoReader.ExpressionParser.Nodes;

public sealed class FunctionArgumentNode : ValueExpressionNode
{
    public FunctionArgumentNode(object objNode) : base(objNode)
    {
        Value = objNode;
    }
    public ValueExpressionNode Calculate(object? rootReflectObject)
    {
        if (Value is string s)
        {
            var rpnExp = RpnTools.ToRpnExpression(s);
            var rslt = RpnTools.CalcRpnStack(rpnExp, rootReflectObject);
            if (double.TryParse(rslt.ToString(), out _))
            {
                return new NumberExpressionNode(rslt.Value);
            }
            if (bool.TryParse(rslt.ToString(), out _))
            {
                return new BoolExpressionNode(rslt.Value);
            }
            return new StringExpressionNode(rslt.Value);
        }
        if (Value == null || Value.Equals(NullNode.Null))
            return NullNode.Null;
        if (double.TryParse(Value.ToString(), out _))
        {
            return new NumberExpressionNode(Value);
        }
        if (bool.TryParse(Value.ToString(), out _))
        {
            return new BoolExpressionNode(Value);
        }
        return new StringExpressionNode(Value);
    }
    public override ExpressionNodeType NodeType => ExpressionNodeType.Argument;
}