using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Tools.I8n;

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

    public class InfoReaderInternalException  : InfoReaderExceptionBase
    {
        public InfoReaderInternalException()
        {
        }

        public InfoReaderInternalException(string msg) : base(msg)
        {
        }

        public InfoReaderInternalException(Exception innerException) : base(
            LocalizationInfo.Current.Translations["LANG_INFO_INTERNAL_ERROR"], innerException)
        {
        }

        public InfoReaderInternalException(string msg, Exception innerException):base(msg, innerException)
        {
        }
    }

    public class CommandInvocationException : InfoReaderExceptionBase
    {
        public CommandInvocationException()
        {
        }

        public CommandInvocationException(string msg) : base(msg)
        {
        }

        public CommandInvocationException(string msg, Exception innerException) : base(msg, innerException)
        {
        }
    }
}
