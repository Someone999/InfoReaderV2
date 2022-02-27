using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;

namespace InfoReader.Tools
{
    internal class MigrationAssistant
    {
        private IniConfigElement? _syncConfigFile;
        private InfoReaderPlugin _plugin;

        public MigrationAssistant(InfoReaderPlugin plugin)
        {
            _plugin = plugin;
            if (!File.Exists("..\\config.ini"))
                return;
            _syncConfigFile = new IniConfigElement("..\\config.ini");
        }

        bool MigrateConfig()
        {
            try
            {
                var pluginCfg = _plugin.ConfigElements["program"];
                var mmfCfg = _plugin.ConfigElements["mmf"];
                if (_syncConfigFile == null)
                {
                    return false;
                }

                var selfCfg = _syncConfigFile["InfoReader"];
                pluginCfg.SetValue("OsuApiKey", selfCfg["ApiKey"]);
                pluginCfg.SetValue("AudioCopyDirectory", selfCfg["DefaultMusicCopyingDirectory"]);
                pluginCfg.SetValue("DebugMode", selfCfg["DebugMode"].GetValue<bool>());
                pluginCfg.SetValue("OverlayDllPath", selfCfg["OverlayDllDir"]);
                pluginCfg.SetValue("VideoCopyDirectory", selfCfg["DefaultVideoCopyingDirectory"]);
                pluginCfg.SetValue("GamePath", selfCfg["GameDir"]);
                pluginCfg.SetValue("BeatmapCopyDirectory", selfCfg["OszDir"]);
                pluginCfg.SetValue("BeatmapReadMethod",
                    new BeatmapReadMethodsConverter().Convert(_syncConfigFile["BeatmapReadMethod"]));
                //cfg.SetValue("AutoUpdate", selfCfg["AutoUpdate"].GetValue<bool>())
                mmfCfg.SetValue("Encoding", new EncodingConverter().Convert(_syncConfigFile["Encoding"]));
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to migrate config.: ");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
