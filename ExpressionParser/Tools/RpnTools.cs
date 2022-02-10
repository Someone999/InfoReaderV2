using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfoReader.ExpressionParser.Nodes;

namespace InfoReader.ExpressionParser.Tools
{
    public static class RpnTools
    {
        public static bool IsOperator(char token)
        {
            char[] ops = { '(', ')', '!', '*', '/', '+', '-', '=', '>', '<', '&', '|' };
            return ops.Contains(token);
        }

        public static bool IsTwoCharacterOperator(char lastToken, char currentToken)
        {
            char[] subOps = { '=', '&', '|' };
            if (IsOperator(lastToken))
            {
                return subOps.Contains(currentToken);
            }
            return false;
        }
        //后缀表达式处理运算符优先级
        /*
         * 用例：
         * 1+5*3*(1+2)
         * 变换方法
         * 1 5 3 1 2 + * * +
         * + *(2) *(1) ( + )
         * 
         * 运算方法
         * 1 5 3 1 2
         * + * * +
         * 1 + 2 = 3  | 1 5 3 3
         *            | + * *
         * 3 * 3 = 9  | 1 5 9
         *            | + *
         * 9 * 5 = 45 | 1 45
         *            | +
         * 1 + 45 = 46 即为结果
         */
        internal static void PriorityProcessor(Stack<OperatorExpressionNode> opNodes, Stack<ExpressionNode> nodes, OperatorExpressionNode op)
        {
            if (op is null || op.Value is null)
                throw new ArgumentNullException();
            if (op.Value.Equals("("))
            {
                opNodes.Push(op);
                return;
            }
            if (op.Value.Equals(")"))
            {
                while (opNodes.Count > 0)
                {
                    var p = opNodes.Pop();
                    if (p.Value != null && !p.Value.Equals("("))
                    {
                        nodes.Push(p);
                    }
                }
                return;
            }

            var topVal = opNodes.Peek().Value;
            while (topVal != null && opNodes.Count > 0 && op.Priority <= opNodes.Peek().Priority && !topVal.Equals("("))
            {
                nodes.Push(opNodes.Pop());
                if (opNodes.Count != 0) 
                    continue;
                opNodes.Push(op);
                return;
            }
            opNodes.Push(op);
        }

        public static Stack<ExpressionNode> ToRpnExpression(string expression,object? funcReflectObj = null, object? rootReflectObj = null, object? valueReflectObj = null)
        {

            Stack<ExpressionNode> nodes = new Stack<ExpressionNode>();
            Stack<OperatorExpressionNode> opNodes = new Stack<OperatorExpressionNode>();
            StringBuilder currentToken = new StringBuilder();
            int lastIndex = -1;
            for (int i = 0; i < expression.Length;)
            {
                bool cleared = false;
                if (char.IsWhiteSpace(expression[i]))
                {
                    i++;
                    continue;
                }

                if (char.IsDigit(expression[i]))
                {
                    if (currentToken.Length > 0 && !double.TryParse(currentToken.ToString(), out _))
                    {
                        currentToken.Append(expression[i++]);
                        continue;
                    }
                    while ((char.IsDigit(expression[i]) || expression[i] == '.') && i < expression.Length)
                    {
                        currentToken.Append(expression[i]);
                        if (i < expression.Length - 1)
                        {
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    string lastToken = currentToken.ToString();
                    if (double.TryParse(lastToken, out double result))
                    {
                        nodes.Push(new NumberExpressionNode(result));
                    }
                    currentToken.Clear();
                    cleared = true;
                    if (i == lastIndex || lastIndex == expression.Length - 1)
                        break;
                }
                if (IsOperator(expression[i]))
                {
                    if (currentToken.Length > 0)
                    {
                        int rb = 0, lb = 0;
                        if (expression[i] == '(')
                        {
                            while (true)
                            {
                                if (expression[i] == '(')
                                {
                                    lb++;
                                }
                                currentToken.Append(expression[i]);
                                if (expression[i] == ')' || i >= expression.Length - 1)
                                {
                                   
                                    rb++;
                                    if(rb == lb)
                                    {
                                        break;
                                    }
                                   
                                }
                                i++;
                            }

                            FunctionParser(nodes, currentToken.ToString(), funcReflectObj, valueReflectObj);
                            currentToken.Clear();
                            cleared = true;
                            continue;
                        }
                        string lastToken = currentToken.ToString();
                        nodes.Push(new IdentifierNode(lastToken));
                    }
                    currentToken.Clear();
                    cleared = true;
                    currentToken.Append(expression[i]);
                    if (i < expression.Length)
                    {
                        i++;
                    }
                    else if (currentToken.ToString() != ")")
                    {
                        throw new ArgumentException("Operator at end of expression");
                    }
                    if (i < expression.Length - 1)
                    {
                        if (IsTwoCharacterOperator(expression[i - 1], expression[i]))
                        {
                            currentToken.Append(expression[i]);
                            i++;
                        }
                    }
                    string op = currentToken.ToString();
                    OperatorExpressionNode opNode = new OperatorExpressionNode(op);
                    PriorityProcessor(opNodes, nodes, opNode);
                    currentToken.Clear();
                    cleared = true;
                    if (i > expression.Length - 1)
                        break;

                    continue;
                }
                if(!cleared)
                    currentToken.Append(expression[i]);
                lastIndex = i;
                i++;
                

            }

            if (currentToken.Length > 0)
            {
                if (currentToken.Length == 1)
                {
                    if (IsOperator(currentToken[0]))
                        throw new ArgumentException("Operator at last place.");
                }
                else if (currentToken.Length == 2)
                {
                    if (IsTwoCharacterOperator(currentToken[0], currentToken[1]))
                        throw new ArgumentException("Operator at last place.");
                }
                if (double.TryParse(currentToken.ToString(), out var rslt))
                {
                    nodes.Push(new NumberExpressionNode(rslt));
                }
                else
                {
                    nodes.Push(new IdentifierNode(currentToken.ToString()));
                }
            }

            while (opNodes.Count > 0)
            {
                nodes.Push(opNodes.Pop());
            }

            return nodes;
        }

        public static ExpressionNode CalcRpnStack(Stack<ExpressionNode> nodes, object? valueReflectObject)
        {
            nodes = new Stack<ExpressionNode>(nodes);
            Stack<ValueExpressionNode> vals = new Stack<ValueExpressionNode>();

            while (nodes.Count > 0)
            {
                if(nodes.Peek() == null)
                {
                    throw new FormatException("Rpn expression is incorrect");
                }
                switch (nodes.Peek())
                {
                    case ValueExpressionNode:
                    {
                        var val = nodes.Pop() as ValueExpressionNode;
                        switch (val)
                        {
                            case null:
                                throw new Exception("Please contact the developer.");
                            case IdentifierNode i:
                                val = i.GetValue(valueReflectObject);
                                break;
                        }

                        vals.Push(val);
                        continue;
                    }
                    case OperatorExpressionNode op:
                    {
                        switch (op.Value)
                        {
                            case null:
                                throw new ArgumentNullException();
                            case "!":
                                vals.Push(op.Calculate(null, vals.Pop()));
                                break;
                            default:
                            {
                                var val2 = vals.Pop();
                                var val1 = vals.Pop();
                                vals.Push(op.Calculate(val1, val2));
                                break;
                            }
                        }

                        nodes.Pop();
                        continue;
                    }
                    case FunctionNode:
                    {
                        var func = (FunctionNode)nodes.Pop();
                        Stack<FunctionArgumentNode> args = new Stack<FunctionArgumentNode>();
                        while (nodes.Peek() is FunctionArgumentNode)
                        {                       
                            args.Push((FunctionArgumentNode)nodes.Pop() );
                        }
                        vals.Push(func.Invoke(args.Reverse().ToArray()) ?? throw new NullReferenceException());
                        continue;
                    }
                }

                
            }
            if (vals.Count > 1)
                throw new Exception("Rpn expression is incorrect");
            return vals.Pop();
        }


        public static void FunctionParser(Stack<ExpressionNode> nodeStack, string expression, object? funcReflectObject = null, object? valueReflectObject = null)
        {
            StringBuilder currentToken = new StringBuilder();
            List<FunctionArgumentNode> argNodes = new List<FunctionArgumentNode>();
            Stack<FunctionNode> functions = new Stack<FunctionNode>();
            int layer = 0;
            for(int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '(')
                {
                    if (currentToken.Length <= 0) 
                        continue;
                    if (layer < 1)
                    {
                        i++;
                        functions.Push(new FunctionNode(currentToken.ToString()));
                        layer++;
                    }
                    else
                    {
                        do
                        {
                            currentToken.Append(expression[i]);
                            i++;
                        } while (expression[i] == ')');

                        FunctionParser(nodeStack, currentToken.ToString(), funcReflectObject, valueReflectObject);
                        layer--;
                    }

                    currentToken.Clear();
                }

                else if (expression[i] == ',')
                {
                    if (currentToken.Length <= 0)
                        continue;
                    i++;
                    argNodes.Add(new FunctionArgumentNode(currentToken.ToString()));
                    currentToken.Clear();
                }

                else if (expression[i] == ')')
                {
                    if (currentToken.Length <= 0)
                        continue;
                    argNodes.Add(new FunctionArgumentNode(currentToken.ToString()));
                    currentToken.Clear();
                }

                currentToken.Append(expression[i]);
            }
            var rslt = functions.Peek().Invoke(argNodes.ToArray(),funcReflectObject, valueReflectObject);
            if(rslt != null)
            {
                nodeStack.Push(rslt);
            }

        }
    }
}
