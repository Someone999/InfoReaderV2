namespace InfoReader.ExpressionParser.Nodes
{
    public class NullNode : ValueExpressionNode
    {
        private NullNode() : base("null")
        {
        }
        public override ExpressionNodeType NodeType => ExpressionNodeType.Null;
        static NullNode? _nullNode;
        public static NullNode Null => _nullNode ??= new NullNode();
    }
}
