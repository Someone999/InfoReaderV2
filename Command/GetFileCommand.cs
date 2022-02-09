﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Command.Parser;
using InfoReader.Tools;
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
            throw new NotImplementedException();
        }

        void InitDirectory(InfoReaderPlugin instance)
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
            string fileName = "", oriFileName = "";
            var beatmap = instance.MemoryDataSource?.Beatmap;
            if (beatmap == null)
            {
                Logger.LogError("No beatmap selected.");
                return true;
            }
            switch (fileType)
            {
                case "video":
                    if (!beatmap.HasVideo)
                    {
                        Logger.LogWarning("Beatmap has no video.");
                        return true;
                    }
                    oriFileName = beatmap.FullVideoFileName;
                    fileName = PathTools.GetBeatmapFileName
                        (instance.Configuration.VideoCopyDirectory, beatmap, Path.GetExtension(beatmap.VideoFileName));
                    break;
                case "audio":
                    oriFileName = beatmap.FullAudioFileName;
                    fileName = PathTools.GetBeatmapFileName
                        (instance.Configuration.AudioCopyDirectory, beatmap, Path.GetExtension(beatmap.AudioFileName));
                    break;
                case "bg":
                    oriFileName = beatmap.FullBackgroundFileName;
                    fileName = PathTools.GetBeatmapFileName
                        (instance.Configuration.BackgroundCopyDirectory, beatmap, Path.GetExtension(beatmap.VideoFileName));
                    break;
                case "osz":
                    OsuInfo info = new();
                    fileName = PathTools.GetBeatmapFileName(instance.Configuration.BeatmapCopyDirectory, beatmap,
                        "osz");
                    string des = Path.Combine(instance.Configuration.BeatmapCopyDirectory,
                        $"{beatmap.BeatmapSetId} {beatmap.Artist}-{beatmap.Title}.osz");
                    ZipFile.CreateFromDirectory
                        (Path.Combine(info.BeatmapDirectory, beatmap.BeatmapFolder), des);
                    Logger.Log("Completed.");
                    return true;
                default: 
                    Logger.LogError("Not supported option.");
                    return true;
            }

            try
            {
                File.Copy(oriFileName, fileName);
                Logger.Log("Completed.");
            }
            catch (Exception e)
            {
                Logger.LogError("Failed.");
                Logger.LogError(e.Message);
                
            }
            return true;
        }
    }
}
