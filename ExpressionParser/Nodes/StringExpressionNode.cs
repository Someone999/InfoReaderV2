namespace InfoReader.ExpressionParser.Nodes
{
    public sealed class StringExpressionNode : ValueExpressionNode
    {
        public StringExpressionNode(object? objNode) : base(objNode)
        {
            Value = objNode?.ToString() ?? string.Empty;
        }

        public override ExpressionNodeType NodeType => ExpressionNodeType.String;
    }
}
