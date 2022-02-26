using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Command.Parser;
using InfoReader.Tools;
using InfoReader.Tools.I8n;

namespace InfoReader.Command
{
    public class HelpCommand:ICommandProcessor
    {
        public bool AutoCatch => true;
        public string MainCommand => "help";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
        private InfoReaderPlugin? _plugin;

        public string GetHelp()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(LocalizationInfo.Current.Translations["LANG_HELP_GETINFO_EXTRA"]);
            builder.AppendLine(LocalizationInfo.Current.Translations["LANG_INFO_AVAILABLECMDS"]);
            foreach (var command in _plugin?.CommandProcessors ?? new Dictionary<string, ICommandProcessor>())
                builder.AppendLine(command.Value.MainCommand);
            return builder.ToString();
        }

        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            _plugin = instance;
            if (parser.Arguments.Length <= 0)
            {
                Logger.Log(GetHelp(),ConsoleColor.White);
                return true;
            }

            var commands = instance.CommandProcessors[parser.Arguments[0]];
            if (commands is not null)
                Logger.Log(commands.GetHelp(),ConsoleColor.White);
            return true;
        }
    }
}
