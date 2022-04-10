using System;
using System.Collections.Generic;
using System.IO;
using InfoReader.ExpressionParser.Nodes;
using Lexer;
using Lexer.Expressions;
using Lexer.RpnExpression;

namespace InfoReader.ExpressionParser.Tools
{
    internal static class InternalMethods
    {
        public static Dictionary<string, IRpnValue> Variables = new();

        public static void AddOrUpdateVariables(Dictionary<string, IRpnValue> vars)
        {
            foreach (var expressionNode in Variables)
            {
                if (Variables.ContainsKey(expressionNode.Key))
                {
                    Variables[expressionNode.Key] = expressionNode.Value;
                    return;
                }
                Variables.Add(expressionNode.Key, expressionNode.Value);
            }
        }
        internal static double Sqrt(double num) => Math.Sqrt(num);
        internal static double Pow(double x, double y) => Math.Pow(x, y);
        internal static double Log(double a, double x) => Math.Log(a, x);
        internal static double Log10(double a) => Math.Log10(a);
        internal static IRpnValue Set(string varName, string val)
        {
            CodeLexer lexer = new CodeLexer(new StringReader(val));
            lexer.Parse();
            CalculationExpression calculationExpression = new CalculationExpression(lexer.Tokens);
            var rslt = calculationExpression.GetRpnValue();
            if (rslt == null)
            {
                throw new ArgumentException();
            }

            if (Variables.ContainsKey(varName))
            {
                Variables[varName] = rslt;
            }
            else
            {
                Variables.Add(varName, rslt);
            }
            return rslt;
        }
    }

}
