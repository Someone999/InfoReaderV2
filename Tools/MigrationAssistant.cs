using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using InfoReader.Configuration;
using InfoReader.Configuration.Elements;
using InfoReader.Exceptions;
using InfoReader.Mmf;
using InfoReader.Tools.I8n;
using osuTools.Game;

namespace InfoReader.Tools
{
    internal class MigrationAssistant
    {
        private IniConfigElement? _syncConfigFile;
        private InfoReaderPlugin _plugin;
        private IConfigElement _pluginConfigElement;

        public MigrationAssistant(InfoReaderPlugin plugin)
        {
            _plugin = plugin;
            _pluginConfigElement = _plugin.Configurables["program"]?.ConfigElement ?? 
                                   throw new InfoReaderInternalException(new InvalidOperationException("Config element is null"));
            if (!File.Exists("..\\..\\config.ini"))
                return;
            _syncConfigFile = new IniConfigElement("..\\..\\config.ini");
        }

        bool MigrateConfig()
        {
            try
            {

                var pluginCfg = _plugin.Configurables["program"].ConfigElement?["Program"] ?? throw new InvalidOperationException();
                var mmfCfg = _plugin.Configurables["mmf"].ConfigElement?["MmfConfigs"] ?? throw new InvalidOperationException();
                if (_syncConfigFile == null)
                {
                    return false;
                }
                Logger.Log("[InfoReader] Migrating settings...");


                //Logger.Log($"LanguageId set to {_syncConfigFile["Sync.DefaultConfiguration"]["Language"].GetValue<string>()}");

                //var selfCfg = _syncConfigFile["InfoReader.Setting"];
                if (!_syncConfigFile.ToDictionary().ContainsKey("InfoReader.Setting"))
                {
                    return true;
                }

                var selfCfg = _syncConfigFile["InfoReader.Setting"];
                //Logger.Log("Reading config...");

                pluginCfg.SetValue("OsuApiKey", selfCfg["ApiKey"].GetValue<string>());
                //Logger.Log($"OsuApiKey set to {selfCfg["ApiKey"].GetValue<string>()}");

                pluginCfg.SetValue("AudioCopyDirectory", selfCfg["DefaultMusicCopyingDirectory"].GetValue<string>());
                //Logger.Log($"AudioCopyDirectory set to {selfCfg["DefaultMusicCopyingDirectory"].GetValue<string>()}");

                pluginCfg.SetValue("DebugMode", selfCfg["DebugMode"].GetValue<bool>());
                //Logger.Log($"DebugMode set to {selfCfg["DebugMode"].GetValue<string>()}");

                pluginCfg.SetValue("OverlayDllPath", selfCfg["OverlayDllDir"].GetValue<string>());
                //Logger.Log($"OverlayDllPath set to {selfCfg["OverlayDllDir"].GetValue<string>()}");

                pluginCfg.SetValue("VideoCopyDirectory", selfCfg["DefaultVideoCopyingDirectory"].GetValue<string>());
                //Logger.Log($"VideoCopyDirectory set to {selfCfg["DefaultVideoCopyingDirectory"].GetValue<string>()}");

                pluginCfg.SetValue("GamePath", selfCfg["GameDir"].GetValue<string>());
                //Logger.Log($"GamePath set to {selfCfg["GameDir"].GetValue<string>()}");

                pluginCfg.SetValue("BeatmapCopyDirectory", selfCfg["OszDir"].GetValue<string>());
                //Logger.Log($"BeatmapCopyDirectory set to {selfCfg["OszDir"].GetValue<string>()}");

                pluginCfg.SetValue("BeatmapReadMethod", selfCfg["BeatmapReadMethod"].GetValue<string>());
                //Logger.Log($"BeatmapReadMethod set to {selfCfg["BeatmapReadMethod"].GetValue<string>()}");

                pluginCfg.SetValue("AutoUpdate", selfCfg["AutoUpdate"].GetValue<bool>());
                mmfCfg.SetValue("Encoding", selfCfg["Encoding"].GetValue<string>());
                return true;
            }
            catch(Exception ex)
            {
                Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_MIGRATION_EXCEPTION"].Format(ex.Message));
                return false;
            }
        }

        bool MigrateMmf()
        {
            try
            {
                var mmfCfg = _plugin.Configurables["mmf"] as MmfConfiguration;
                if (mmfCfg == null)
                {
                    return false;
                }
                var mmfs = mmfCfg.MmfList ??= new List<MmfBase>();
                
                Dictionary<OsuGameStatus, string> mmfList = new()
                {
                    { OsuGameStatus.Playing, "InfoReaderMmfPlaying" },
                    { OsuGameStatus.SelectSong, "InfoReaderMmfSelectSong" },
                    { OsuGameStatus.Idle, "InfoReaderMmfMainMenu" },
                    { OsuGameStatus.MatchSetup, "InfoReaderMmfGameRoom" },
                    { OsuGameStatus.Lobby, "InfoReaderMmfLobby" },
                    { OsuGameStatus.Editing, "InfoReaderMmfEditing" },
                    { OsuGameStatus.Rank, "InfoReaderMmfResult" }
                };
                FileTools.ConfirmDirectory("MmfFormats\\");
                foreach (var mmf in mmfList)
                {
                    if (!File.Exists($"MmfFormats\\{mmf.Key}FormatConfig.ifrfmt"))
                    {
                        if (File.Exists($"..\\FormatInfo\\{mmf.Key}FormatConfig.ini"))
                        {
                            File.Move($"..\\FormatInfo\\{mmf.Key}FormatConfig.ini",
                                $"MmfFormats\\{mmf.Key}FormatConfig.ifrfmt");
                        }
                        else
                        {
                            File.Create($"MmfFormats\\{mmf.Key}FormatConfig.ifrfmt").Close();
                        }
                        
                    }
                    
                    StatusMmf statusMmf = new StatusMmf(mmf.Value, mmf.Key)
                    {
                        FormatFile = $"MmfFormats\\{mmf.Key}FormatConfig.ifrfmt",
                        Enabled = true
                    };

                    //Logger.Log($"Mmf {statusMmf.Name} is created.");
                    
                    var mmfMgr = MmfManager.GetInstance(_plugin);
                    if(mmfMgr.Mmfs.All(mmfBase => mmfBase.Name != statusMmf.Name))
                    {
                        mmfMgr.Add(statusMmf);
                        mmfs.Add(statusMmf);
                    }
                    
                }

                if (Directory.Exists("..\\FormatInfo\\"))
                {
                    FileTools.RecurseDelete("..\\FormatInfo\\");
                    Directory.Delete("..\\FormatInfo\\");
                }

                _plugin.Configurables["mmf"].Save();
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_MIGRATION_EXCEPTION"].Format(e.Message));
                return false;
            }
        }

        internal bool Migrate()
        {
            var pluginCfg = _pluginConfigElement["Program"];
            if (_syncConfigFile != null)
            {
                pluginCfg.SetValue("LanguageId", _syncConfigFile["Sync.DefaultConfiguration"]["Language"].GetValue<string>());
            }
            var migrationCfg = _pluginConfigElement["Migration"] ?? throw new InfoReaderInternalException
                (new KeyNotFoundException("Migration setting is missing."));
            if (migrationCfg["IsMigrated"].GetValue<bool>())
            {
                return true;
            }
            bool cfg = MigrateConfig(), mmf = MigrateMmf();
            migrationCfg.SetValue("IsMigrated", true);
            if (_pluginConfigElement is IConfigWriter writer)
            {
                writer.WriteToFile(DefaultFilePath.CurrentConfigFile);
            }
            Logger.LogNotification(LocalizationInfo.Current.Translations["LANG_INFO_MIGRATED"]);
            Thread.Sleep(3000);
            Sync.SyncHost.Instance.RestartSync();
            return cfg && mmf;
        }

    }
}
