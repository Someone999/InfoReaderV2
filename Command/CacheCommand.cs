using System;
using System.Collections.Generic;
using InfoReader.Command.Parser;
using InfoReader.Tools;
using InfoReader.Tools.I8n;
using osuTools.MemoryCache;
using osuTools.MemoryCache.Beatmap;
using osuTools.OrtdpWrapper;

namespace InfoReader.Command
{
    public class CacheCommand:ICommandProcessor
    {
        private static readonly Dictionary<string, Type> NameTypeMap = new Dictionary<string, Type>
        {
            {"beatmap", typeof(MemoryBeatmapCollection)}
        };

        public bool AutoCatch => true;
        public string MainCommand => "cache";
        public bool OnUnhandledException(InfoReaderPlugin instance, Exception exception) => false;
        
        public string GetHelp()
        {
            return LocalizationInfo.Current.Translations["LANG_HELP_CACHE_COMMAND"];
        }

        void ListCache(IMemoryCache cache) => Logger.Log(cache.GetCacheString());
        void ClearCache(IMemoryCache cache) => cache.Clear();
        void GetCacheCount(IMemoryCache cache) => Logger.Log(cache.GetCacheSize().ToString());
        public bool Execute(InfoReaderPlugin instance, CommandParser parser)
        {
            var args = parser.Arguments;
            OrtdpWrapper? wrapper = (OrtdpWrapper?) instance.MemoryDataSource;
            if (wrapper == null)
                return true;
            if (args.Length == 2)
            {
                var cacheDict = wrapper.MemoryCaches;
                string op = args[0];
                string obj = args[1];
                if (!NameTypeMap.ContainsKey(obj))
                {
                    Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_CACHE_TYPE_NOT_EXISTS"]);
                    return true;
                }
                IMemoryCache cacheObj = cacheDict[NameTypeMap[obj]];
                switch (op)
                {
                    case "list": 
                        ListCache(cacheObj);
                        break;
                    case "clear":
                        ClearCache(cacheObj);
                        break;
                    case "count":
                        GetCacheCount(cacheObj);
                        break;
                    default: 
                        Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_CACHE_TYPE_NOT_EXISTS"]);
                        break;
                }

                return true;
            }
            Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_ARG_COUNT_MISMATCHED"]);
            return true;
        }
    }
}
