using System;
using System.Reflection;

namespace InfoReader.ExpressionParser.Nodes
{
    [Obsolete("This class is replaced by Lexer.CodeLexer and Lexer.Expressions.CalculationExpression")]
    public abstract class ExpressionNode
    {
        protected ExpressionNode(object? objNode)
        {
            if (objNode is not null)
            {
                _internalValue = objNode;
            }
        }
        public abstract ExpressionNodeType NodeType { get; }
        private object? _internalValue;
        public virtual object? Value
        {
            get => _internalValue ?? NullNode.Null;
            set
            {
                if (value != null)
                {
                    _internalValue = value;
                }
            }
        } 

        public override string ToString()
        {
            return Value?.ToString() ?? "null";
        }

        public string ToString(string format)
        {
            Type t = Value?.GetType() ?? throw new TypeLoadException();
            MethodInfo? methodInfo = t.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null);
            if (methodInfo is null)
            {
                return ToString();
            }

            return methodInfo.Invoke(Value, new object[] { format }) as string ?? ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            Type t = Value?.GetType() ?? throw new TypeLoadException();
            MethodInfo? methodInfo = t.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string),typeof(IFormatProvider) }, null);
            if (methodInfo is null)
            {
                return ToString(format);
            }
            return methodInfo.Invoke(Value, new object[] { format, formatProvider }) as string ?? ToString();
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(obj, this))
                return true;
            if (obj is ExpressionNode node)
            {
                if (node.Value is ExpressionNode n)
                    throw new InvalidOperationException();
                if (node.Value == null && Value == null)
                {
                    return true;
                }
                if (node.Value == null || Value == null)
                {
                    return false;
                }
                return node.Value.Equals(obj);
            }
            return false;
        }
        public static bool operator ==(ExpressionNode? a, ExpressionNode? b)
        {
            if((object?)a == null && (object?)b == null)
            {
                return true;
            }

            if ((object?)a == null || (object?)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }
        public static bool operator !=(ExpressionNode? a, ExpressionNode? b)
        {          
            if((object?)a == null && (object?)b == null)
            {
                return false;
            }

            if ((object?)a == null || (object?)b == null)
            {
                return true;
            }

            return a.Equals(b);
        }
        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }

}