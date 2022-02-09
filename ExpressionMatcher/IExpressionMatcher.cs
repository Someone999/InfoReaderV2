using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InfoReader.ExpressionMatcher
{
    public interface IExpressionMatcher
    {
        string[] Match(string input);
    }

    public class ValueExpressionMatcher: IExpressionMatcher
    {
        private Regex _valMatcher = new(@"\$\{(.+?)\}");
        public string[] Match(string input)
        {
            List<string> vals = new();
            MatchCollection matches = _valMatcher.Matches(input);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    vals.Add(match.Value);
                }
            }
            return vals.ToArray();
        }
    }
}
