using System;

namespace InfoReader.Tools
{
    public static class Logger
    {
        public static ConsoleColor DefaultColor => ConsoleColor.Gray;

        public static void LogError(string msg, bool newLine = true, bool logTime = true)
        {
            Sync.Tools.IO.CurrentIO.WriteColor(msg, ConsoleColor.Red, newLine, logTime);
        }

        public static void LogWarning(string msg, bool newLine = true, bool logTime = true)
        {
            Sync.Tools.IO.CurrentIO.WriteColor(msg, ConsoleColor.Yellow, newLine, logTime);
        }

        public static void LogNotification(string msg, bool newLine = true, bool logTime = true)
        {
            Sync.Tools.IO.CurrentIO.WriteColor(msg, ConsoleColor.DarkGreen, newLine, logTime);
        }

        public static void Log(string msg, ConsoleColor color = ConsoleColor.White, bool newLine = true, bool logTime = true)
        {
            Sync.Tools.IO.CurrentIO.WriteColor(msg, color, newLine, logTime);
        }
    }
}
