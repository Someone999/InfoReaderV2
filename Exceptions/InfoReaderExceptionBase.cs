using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Exceptions
{
    public abstract class InfoReaderExceptionBase : Exception
    {
        protected InfoReaderExceptionBase(string msg, Exception innerException):base(msg, innerException)
        {
        }

        protected InfoReaderExceptionBase(string msg) : base(msg)
        {
        }

        protected InfoReaderExceptionBase()
        {
        }
    }
}
