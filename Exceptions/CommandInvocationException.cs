using System;

namespace InfoReader.Exceptions;

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