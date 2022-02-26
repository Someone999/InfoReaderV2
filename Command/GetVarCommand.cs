using System;
using InfoReader.Command.Parser;
using InfoReader.ExpressionParser.Tools;
using InfoReader.Tools;
using InfoReader.Tools.I8n;

namespace InfoReader.Command
{
    public class GetVarCommand : ICommandProcessor
    {
        public bool AutoCatch => true;
        public string MainCommand => "var";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception)
        {
            return false;
        }

        public string GetHelp()
        {
            return LocalizationInfo.Current.Translations["LANG_HELP_VARGETTER"];
        }

        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            foreach (var arg in parser.Arguments)
            {
                var val = RpnTools.CalcRpnStack(RpnTools.ToRpnExpression(arg), instance.MemoryDataSource).Value;
                    Logger.Log(val?.ToString() ?? throw new ArgumentException());
            }
            return true;
        }

        
    }
}
