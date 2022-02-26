using System;
using System.Linq;
using System.Reflection;
using InfoReader.ExpressionParser.Tools;

namespace InfoReader.ExpressionParser.Nodes
{
    public sealed class FunctionNode : ExpressionNode
    {
        public FunctionNode(object objNode) : base(objNode)
        {
            if (objNode is string)
            {
                Value = objNode;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public ValueExpressionNode? Invoke(FunctionArgumentNode[] args, object? rootReflectObject = null,object? valueReflectObject= null)
        {

            Type t = rootReflectObject?.GetType() ?? typeof(InternalMethods);
            MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            MethodInfo? method = methods.FirstOrDefault(m => m.Name == Value?.ToString());
            if(method == null)
            {
                return null;
            }
            object[] realArgs = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var realArg = args[i].Calculate(valueReflectObject).Value;
                if (realArg != null)
                {
                    realArgs[i] = realArg;
                }
            }
            var ret = method.Invoke(rootReflectObject, realArgs);
            if(ret == null)
            {
                return null;
            }
            if(double.TryParse(ret.ToString(),out var numVal))
            {
                return new NumberExpressionNode(numVal);
            }
            if (bool.TryParse(ret.ToString(), out var boolVal))
            {
                return new BoolExpressionNode(boolVal);
            }
            return new StringExpressionNode(ret);

        }

        public override ExpressionNodeType NodeType => ExpressionNodeType.Function;
    }
}
