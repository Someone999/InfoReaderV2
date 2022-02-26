using System;
using System.Collections.Generic;
using InfoReader.ExpressionParser.Nodes;

namespace InfoReader.ExpressionParser.Tools
{
    internal static class InternalMethods
    {
        public static Dictionary<string, ExpressionNode> Variables = new();

        public static void AddOrUpdateVariables(Dictionary<string, ExpressionNode> vars)
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
        internal static ExpressionNode Set(string varName, string val)
        {
            var rslt = RpnTools.CalcRpnStack(RpnTools.ToRpnExpression(val), Variables);
            if (rslt == NullNode.Null)
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
