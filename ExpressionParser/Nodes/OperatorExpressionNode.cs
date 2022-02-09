using System;

namespace InfoReader.ExpressionParser.Nodes
{
    public sealed class OperatorExpressionNode : ExpressionNode
    {
        public static int GetPriority(string op)
        {
            switch (op)
            {
                case "(":
                case ")": return 5;
                case "!": return 4;
                case "*":
                case "/": return 3;
                case "+":
                case "-": return 2;
                case "==":
                case ">=":
                case "<=":
                case ">":
                case "<":
                case "!=": return 1;
                case "&&":
                case "||": return 0;
                default: throw new ArgumentException($"Invalid operator {(string.IsNullOrEmpty(op) ? "(null or empty)" : op)}.");//Incorrect Operator.
            }
        }
        public OperatorExpressionNode(object objNode) : base(objNode)
        {
            Value = objNode;
        }
        public override ExpressionNodeType NodeType => ExpressionNodeType.Operator;
        int _priority = -1;
        public int Priority => _priority == -1 ? _priority = GetPriority(Value as string ?? throw new InvalidCastException()) : _priority;
        public ValueExpressionNode Calculate(ValueExpressionNode? left, ValueExpressionNode right)
        {
            if (Value is not string s) 
                throw new InvalidOperationException();
            if (left != null)
            {
                CheckNull(left, right);
                return s switch
                {
                    "+" => Add((NumberExpressionNode) left, (NumberExpressionNode) right),
                    "-" => Sub((NumberExpressionNode) left, (NumberExpressionNode) right),
                    "*" => Mul((NumberExpressionNode) left, (NumberExpressionNode) right),
                    "/" => Div((NumberExpressionNode) left, (NumberExpressionNode) right),
                    ">=" => GreaterOrEquals((NumberExpressionNode) left, (NumberExpressionNode) right),
                    "<=" => LessOrEquals((NumberExpressionNode) left, (NumberExpressionNode) right),
                    ">" => Greater((NumberExpressionNode) left, (NumberExpressionNode) right),
                    "<" => Less((NumberExpressionNode) left, (NumberExpressionNode) right),
                    "==" => Equals(left, right),
                    "!=" => NotEquals(left, right),
                    "&&" => BoolAnd((BoolExpressionNode) left, (BoolExpressionNode) right),
                    "||" => BoolOr((BoolExpressionNode) left, (BoolExpressionNode) right),
                    _ => throw new InvalidOperationException()
                };
            }

            if(s == "!")
            {
                return Not((BoolExpressionNode)right);
            }
            throw new InvalidOperationException();
        }

        void CheckNull(params ExpressionNode?[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node == null || node.Value == null)
                    throw new ArgumentNullException();
            }
        }
#pragma warning disable 8605
        NumberExpressionNode Add(NumberExpressionNode a, NumberExpressionNode b)
        {
            return new NumberExpressionNode((double)a.Value + (double)b.Value);
        }

        NumberExpressionNode Sub(NumberExpressionNode a, NumberExpressionNode b)
        {
            return new NumberExpressionNode(-(double)a.Value + (double)b.Value);
        }

        NumberExpressionNode Mul(NumberExpressionNode a, NumberExpressionNode b)
        {
            return new NumberExpressionNode((double)a.Value * (double)b.Value);
        }

        NumberExpressionNode Div(NumberExpressionNode a, NumberExpressionNode b)
        {
            return new NumberExpressionNode((double)a.Value / (double)b.Value);
        }

        BoolExpressionNode Equals(ValueExpressionNode a, ValueExpressionNode b)
        {
            return new BoolExpressionNode(a.Equals(b));
        }
        BoolExpressionNode NotEquals(ValueExpressionNode a, ValueExpressionNode b)
        {
            return new BoolExpressionNode(!a.Equals(b));
        }

        BoolExpressionNode BoolAnd(BoolExpressionNode a, BoolExpressionNode b)
        {
            return new BoolExpressionNode((bool)a.Value && (bool)b.Value);
        }

        BoolExpressionNode BoolOr(BoolExpressionNode a, BoolExpressionNode b)
        {
            return new BoolExpressionNode((bool)a.Value || (bool)b.Value);
        }

        BoolExpressionNode Greater(NumberExpressionNode a, NumberExpressionNode b)
        {
            return new BoolExpressionNode((double)a.Value > (double)b.Value);
        }

        BoolExpressionNode Less(NumberExpressionNode a, NumberExpressionNode b)
        {
            return new BoolExpressionNode((double)a.Value < (double)b.Value);
        }

        BoolExpressionNode GreaterOrEquals(NumberExpressionNode a, NumberExpressionNode b)
        {
            return new BoolExpressionNode((double)a.Value >= (double)b.Value);
        }

        BoolExpressionNode LessOrEquals(NumberExpressionNode a, NumberExpressionNode b)
        {
            return new BoolExpressionNode((double)a.Value <= (double)b.Value);
        }

        BoolExpressionNode Not(BoolExpressionNode right)
        {
            return new BoolExpressionNode(!(bool)right.Value);
        }
#pragma warning restore 8605
    }

}