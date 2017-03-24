using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BrawlhallaOverlay.Ping;
using BrawlhallaOverlay.Damage;
using Newtonsoft.Json;

namespace BrawlhallaOverlay
{
    public class Config
    {
        public PingConfig PingConfig { get; set; }
        public DamageConfig DamageConfig { get; set; }
    }

    public static class ConfigManager
    {
        private static string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BrawlhallaPingOverlay\";

        private static JsonSerializer _serializer = new JsonSerializer();

        private static Config _config;

        public static PingConfig DefaultPingConfig
        {
            get
            {
                return new PingConfig
                {
                    OverlayEnabled = false,
                    ServersEnabled = new List<Server>(),
                    GreyBackground = false,
                    PingOutline = true,
                    PingFontSize = 20,
                    LowPingColor = Utilities.GetDefaultPingColor("low ping"),
                    MediumPingColor = Utilities.GetDefaultPingColor("medium ping"),
                    HighPingColor = Utilities.GetDefaultPingColor("high ping")
                };
            }
        }
        public static DamageConfig DefaultDamageConfig
        {
            get
            {
                return new DamageConfig()
                {

                };
            }
        }
        public static Config DefaultConfig
        {
            get
            {
                return new Config()
                {
                    PingConfig = DefaultPingConfig,
                    DamageConfig = DefaultDamageConfig
                };
            }
        }

        public static PingConfig GetPingConfig()
        {
            if (_config == null)
            {
                _config = LoadConfig();
            }
            return _config.PingConfig;
        }
        public static DamageConfig GetDamageConfig()
        {
            if (_config == null)
            {
                _config = LoadConfig();
            }
            return _config.DamageConfig;
        }

        public static Config LoadConfig()
        {
            // If the config does not exist
            if (!File.Exists(ConfigPath + "config.json"))
            {
                return DefaultConfig;
            }
            try
            {
                using (var reader = new StreamReader(ConfigPath + "config.json"))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var config = _serializer.Deserialize<Config>(jsonReader);
                        if (config == null || config.PingConfig == null || config.DamageConfig == null) // config file exists but is empty or corrupted
                        {
                            return DefaultConfig;
                        }
                        return config;
                    }
                }
            }
            catch
            {
                MessageBox.Show("An error occured while reading configs.", ":(", MessageBoxButton.OK, MessageBoxImage.Error);
                return DefaultConfig;
            }
        }

        public static void SaveConfig()
        {
            var fileInfo = new FileInfo(ConfigPath);
            fileInfo.Directory.Create(); // Creates the directory if it does not exist

            using (var streamWriter = new StreamWriter(ConfigPath + "config.json", false))
            {
                _serializer.Serialize(streamWriter, _config);
            }
        }
    }
}
