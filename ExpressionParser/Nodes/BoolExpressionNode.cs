using System;

namespace InfoReader.ExpressionParser.Nodes
{
    public sealed class BoolExpressionNode : ValueExpressionNode
    {
        public BoolExpressionNode(object? objNode) : base(objNode)
        {
            Value = Convert.ToBoolean(objNode);
        }

        public override ExpressionNodeType NodeType => ExpressionNodeType.Bool;
    }

}