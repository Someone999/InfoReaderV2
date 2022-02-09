using System;
using InfoReader.Command.Parser;

namespace InfoReader.Command
{
    public class ConfigCommand:ICommandProcessor
    {
        public bool AutoCatch => true;
        public string MainCommand => "config";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
        

        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        
        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            var args = parser.Arguments;
            return true;
        }
    }
}
