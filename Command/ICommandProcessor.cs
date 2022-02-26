using System;
using InfoReader.Command.Parser;

namespace InfoReader.Command
{
    public interface ICommandProcessor
    {
        bool AutoCatch { get; }
        string MainCommand { get; }
        bool OnUnhandledException(InfoReaderPlugin instance, Exception exception);
        string GetHelp();
        bool Execute(InfoReaderPlugin instance, CommandParser parser);
        
    }
}
