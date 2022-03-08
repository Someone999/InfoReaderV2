using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Exceptions
{
    public class JsonLexerException : JsonParseException
    {
        public JsonLexerException(string msg):base(msg)
        {
        }
    }
}
