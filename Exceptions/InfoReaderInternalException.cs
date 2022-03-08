using System;
using InfoReader.Tools.I8n;

namespace InfoReader.Exceptions;

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