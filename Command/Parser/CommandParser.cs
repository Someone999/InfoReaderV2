using System.Collections.Generic;

namespace InfoReader.Command.Parser
{
    public class CommandParser
    {
        public CommandParser(string[] command)
        {
            if (command.Length <= 0)
                return;
            MainCommand = command[0];
            for (int i = 1; i < command.Length; i++)
            {
                _argsList.Add(command[i]);
            }
        }

        readonly List<string> _argsList = new();
        public string MainCommand { get; } = string.Empty;

        public string[] Arguments => _argsList.ToArray();
    }
}
