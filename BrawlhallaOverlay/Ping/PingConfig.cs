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