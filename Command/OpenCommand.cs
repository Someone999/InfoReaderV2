using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Command.Parser;
using InfoReader.Tools;

namespace InfoReader.Command
{
    public class OpenCommand: ICommandProcessor
    {
        public bool AutoCatch => true;
        public string MainCommand => "open";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
       
        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        public static void Open(string target) => ProcessTools.StartProcess(target);
       

        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            var args = parser.Arguments;
            if (args.Length == 0)
                return true;
            switch (args[0])
            {
                case "oszfolder": 
                    Open(instance.Configuration.BeatmapCopyDirectory);
                    break;
                case "musicfolder":
                    Open(instance.Configuration.AudioCopyDirectory);
                    break;
                case "videofolder":
                    Open(instance.Configuration.VideoCopyDirectory);
                    break;
                case "bgfolder":
                    Open(instance.Configuration.BackgroundCopyDirectory);
                    break;
                case "beatmappage":
                    var link = instance.MemoryDataSource?.DownloadLink;
#pragma warning disable
                    if (!string.IsNullOrEmpty(link))
                    {
                        Open(link);
                    }
#pragma warning restore
                    break;
            }

            return true;
        }
    }
}
