using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Windows.Media;

namespace BrawlhallaOverlay.Ping
{
    public class PingConfig
    {
        public static JsonSerializer jsonSerializer = new JsonSerializer();

        private static Config _config;
        private static string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BrawlhallaPingOverlay\";

        public static Config Default
        {
            get
            {
                return new Config
                {
                    OverlayEnabled = false,
                    ServersEnabled = new List<Server>(),
                    GreyBackground = false,
                    PingOutline = true,
                    PingFontSize = 20,
                    LowPingColor = PingConfig.GetDefaultColor("low ping"),
                    MediumPingColor = PingConfig.GetDefaultColor("medium ping"),
                    HighPingColor = PingConfig.GetDefaultColor("high ping")
                };
            }
        }

        public static Config GetConfig()
        {
            if (_config == null)
            {
                _config = LoadConfig();
            }
            return _config;
        }

        public static Config LoadConfig()
        {
            //If the config does not exist
            if (!File.Exists(Path + "config.json"))
            {
                return PingConfig.Default;
            }
            try
            {
                using (var reader = new StreamReader(Path + "config.json"))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var config = jsonSerializer.Deserialize<Config>(jsonReader);
                        if (config == null) // config file exists but is empty
                        {
                            return PingConfig.Default;
                        }
                        return config;
                    }
                }
            }
            catch
            {
                MessageBox.Show("An error occured while reading configs.", ":(", MessageBoxButton.OK, MessageBoxImage.Error);
                return PingConfig.Default;
            }
        }

        public static void EditServersEnabled(List<PingItem> servers)
        {
            var config = GetConfig();

            if (config.OverlayEnabled) // Make sure the overlay is enabled so we don't end up clearing the list
            {
                config.ServersEnabled.Clear();

                foreach (var server in servers)
                {
                    config.ServersEnabled.Add(new Server(server.Server, server.XPos, server.YPos));
                }
            }
        }

        public static void SaveConfig()
        {
            var fileInfo = new FileInfo(Path);
            fileInfo.Directory.Create(); // Creates the directory if it does not exist

            using (var streamWriter = new StreamWriter(Path + "config.json", false))
            {
                jsonSerializer.Serialize(streamWriter, GetConfig());
            }
        }

        public static Color GetDefaultColor(string pingBoundary)
        {
            switch (pingBoundary.ToLower())
            {
                case "low ping":
                    return Brushes.Green.Color;
                case "medium ping":
                    return Brushes.Yellow.Color;
                case "high ping":
                    return Brushes.Red.Color;
                default:
                    throw new ArgumentException("Invalid ping boundary.");
            }
        }
    }

    public class Config
    {
        public bool OverlayEnabled { get; set; }
        public List<Server> ServersEnabled { get; set; }

        public bool GreyBackground { get; set; }
        public bool PingOutline { get; set; }
        public int PingFontSize { get; set; }

        public Color LowPingColor
        {
            get
            {
                return _lowPingColor;
            }
            set
            {
                _lowPingColor = value;
                LowPingBrush = new SolidColorBrush(value);
            }
        }
        public Color MediumPingColor
        {
            get
            {
                return _mediumPingColor;
            }
            set
            {
                _mediumPingColor = value;
                MediumPingBrush = new SolidColorBrush(value);
            }
        }
        public Color HighPingColor
        {
            get
            {
                return _highPingColor;
            }
            set
            {
                _highPingColor = value;
                HighPingBrush = new SolidColorBrush(value);
            }
        }

        [NonSerialized]
        private Color _lowPingColor;
        [NonSerialized]
        private Color _mediumPingColor;
        [NonSerialized]
        private Color _highPingColor;

        [NonSerialized]
        public Brush LowPingBrush;
        [NonSerialized]
        public Brush MediumPingBrush;
        [NonSerialized]
        public Brush HighPingBrush;
    }

    public class Server
    {
        public string Name { get; set; }
        public string PingLocation { get; set; }
        public double xPos { get; set; }
        public double yPos { get; set; }

        public Server(string name, double x, double y)
        {
            this.Name = name;
            this.xPos = x;
            this.yPos = y;

            PingLocation = Utilities.GetIPToPingFromName(name);
        }
    }
}