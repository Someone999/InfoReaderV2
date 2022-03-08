using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Exceptions
{
    public class JsonDeserializerException : JsonParseException
    {
        public JsonDeserializerException(string msg) : base(msg)
        {
        }
    }
}
