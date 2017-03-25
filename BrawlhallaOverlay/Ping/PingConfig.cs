using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace BrawlhallaOverlay.Ping
{
    public class PingConfig
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

        [JsonIgnore]
        private Color _lowPingColor;
        [JsonIgnore]
        private Color _mediumPingColor;
        [JsonIgnore]
        private Color _highPingColor;

        [JsonIgnore]
        public Brush LowPingBrush;
        [JsonIgnore]
        public Brush MediumPingBrush;
        [JsonIgnore]
        public Brush HighPingBrush;
    }

    public class Server
    {
        public string Name { get; set; }
        public double XPos { get; set; }
        public double YPos { get; set; }

        public Server(string name, double x, double y)
        {
            this.Name = name;
            this.XPos = x;
            this.YPos = y;
        }
    }
}