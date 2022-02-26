using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InfoReader.Command.Parser;
using InfoReader.Tools;
using InfoReader.Tools.I8n;
using osuTools.GameInfo;

namespace InfoReader.Command
{
    public class GetFileCommand : ICommandProcessor
    {
        public bool AutoCatch => true;
        public string MainCommand => "get";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
        public string GetHelp()
        {
            return LocalizationInfo.Current.Translations["LANG_HELP_FILEGETTER"];
        }

        private void InitDirectory(InfoReaderPlugin instance)
        {
            if (!Directory.Exists(instance.Configuration.AudioCopyDirectory))
            {
                Directory.CreateDirectory(instance.Configuration.AudioCopyDirectory);
            }
            if (!Directory.Exists(instance.Configuration.BackgroundCopyDirectory))
            {
                Directory.CreateDirectory(instance.Configuration.BackgroundCopyDirectory);
            }
            if (!Directory.Exists(instance.Configuration.VideoCopyDirectory))
            {
                Directory.CreateDirectory(instance.Configuration.VideoCopyDirectory);
            }
            if (!Directory.Exists(instance.Configuration.BeatmapCopyDirectory))
            {
                Directory.CreateDirectory(instance.Configuration.BeatmapCopyDirectory);
            }
        }
        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            InitDirectory(instance);
            var args = parser.Arguments;
            if (args.Length == 0)
            {
                return true;
            }

            var fileType = args[0];
            string fileName, oriFileName;
            var beatmap = instance.MemoryDataSource?.Beatmap;
            if (beatmap == null)
            {
                Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_NOBEATMAP"]);
                return true;
            }
            switch (fileType)
            {
                case "video":
                    if (!beatmap.HasVideo)
                    {
                        Logger.LogWarning(LocalizationInfo.Current.Translations["LANG_ERR_NOVIDEO"]);
                        return true;
                    }
                    oriFileName = beatmap.FullVideoFileName;
                    fileName = PathTools.GetFileName
                        (instance.Configuration.VideoCopyDirectory, beatmap, Path.GetExtension(beatmap.VideoFileName));
                    break;
                case "audio":
                    oriFileName = beatmap.FullAudioFileName;
                    fileName = PathTools.GetFileName
                        (instance.Configuration.AudioCopyDirectory, beatmap, Path.GetExtension(beatmap.AudioFileName));
                    break;
                case "bg":
                    oriFileName = beatmap.FullBackgroundFileName;
                    fileName = PathTools.GetFileName
                        (instance.Configuration.BackgroundCopyDirectory, beatmap, Path.GetExtension(beatmap.VideoFileName));
                    break;
                case "osz":
                    OsuInfo info = new();
                    fileName = PathTools.GetFileName(instance.Configuration.BeatmapCopyDirectory, beatmap,
                        "osz");
                    string des = Path.Combine(instance.Configuration.BeatmapCopyDirectory, fileName);
                    ZipFile.CreateFromDirectory
                        (Path.Combine(info.BeatmapDirectory, beatmap.BeatmapFolder), des);
                    Logger.Log(LocalizationInfo.Current.Translations["LANG_INFO_COMPRESSED"]);
                    return true;
                default: 
                    Logger.LogError("Not supported option.");
                    return true;
            }

            try
            {
                if (File.Exists(fileName))
                {
                    bool endsWithDashNumber = PathTools.FileNumberMatcher.IsMatch(fileName);
                    int id = PathTools.GetFileNumber(fileName, endsWithDashNumber);
                    fileName = PathTools.AddNumber(fileName, 0);
                }
                File.Copy(oriFileName, fileName);
                Logger.Log(LocalizationInfo.Current.Translations["LANG_INFO_COPYSUCCESS"]);
            }
            catch (Exception e)
            {
                Logger.LogError(LocalizationInfo.Current.Translations["LANG_INFO_COPYFAILED"]);
                Logger.LogError(e.Message);
                
            }
            return true;
        }
    }
}
