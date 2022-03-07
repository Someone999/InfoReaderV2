namespace InfoReader.ExpressionParser.Nodes
{
    public static class ExtraMethods
    {
        public static IdentifierNode GetAlias(this IdentifierNode node, InfoReaderPlugin plugin)
        {
            if (node.Value == null)
            {
                return new IdentifierNode("");
            }
            return plugin.Variables.ContainsKey(node.Value.ToString()) ? new IdentifierNode(plugin.Variables[node.Value.ToString()].Name) : node;
        }
    }
}
