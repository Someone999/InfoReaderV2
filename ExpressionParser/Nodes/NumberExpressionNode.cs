using System;

namespace InfoReader.ExpressionParser.Nodes
{
    public sealed class NumberExpressionNode : ValueExpressionNode
    {
        public NumberExpressionNode(object? objNode) : base(objNode)
        {
            Value = Convert.ToDouble(objNode);
        }
        public override ExpressionNodeType NodeType => ExpressionNodeType.Number;

    }

}