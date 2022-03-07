using System;
using System.Collections.Generic;
using System.Text;

namespace InfoReader.Tools
{
    public class UrlBuilder
    {
        private List<string> _dir = new List<string>();
        private Dictionary<string, object> _args = new Dictionary<string, object>();
        private string _baseUrl;
        public UrlBuilder(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public UrlBuilder Add(string dirName)
        {
            _dir.Add(dirName);
            return this;
        }

        public UrlBuilder AddArgument(string key, object value)
        {
            _args.Add(key, value.ToString());
            return this;
        }

        public string Generate()
        {
            StringBuilder builder = new StringBuilder(_baseUrl);
            if (!_baseUrl.EndsWith("/"))
            {
                builder.Append('/');
            }
            foreach (var name in _dir)
            {
                string tmpName = name;
                if (name.StartsWith("/") || name.StartsWith("\\"))
                {
                    tmpName = tmpName.Substring(1);
                }
                builder.Append(tmpName);
                builder.Append('/');
            }

            builder.Remove(builder.Length - 1, 1);
            if (_args.Count > 0)
            {
                builder.Append("?");
                foreach (var arg in _args)
                {
                    builder.Append($"{arg.Key}={arg.Value}&");
                }

                builder.Remove(builder.Length - 1, 1);
            }

            string notEscaped = builder.ToString();
            return Uri.EscapeUriString(notEscaped);
        }

    }
}
