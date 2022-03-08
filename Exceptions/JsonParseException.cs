using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Exceptions
{
    public abstract class JsonParseException : InfoReaderExceptionBase
    {
        protected JsonParseException(string msg) : base(msg)
        {
        }
    }
}
