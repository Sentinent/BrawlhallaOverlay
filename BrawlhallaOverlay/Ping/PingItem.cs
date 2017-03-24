using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BrawlhallaOverlay.Overlay;

namespace BrawlhallaOverlay.Ping
{
    public class PingAverages
    {
        public int AveragePing => (int)(pings.Average());

        private List<int> pings = new List<int>();

        public void Add(int ping)
        {
            // Limit our amount of pings to 3
            if (pings.Count > 3)
            {
                pings.RemoveRange(3, pings.Count - 3);
            }
            pings.Add(ping);
        }
    }

    //Represents a block on the overlay
    public class PingItem : OverlayItem
    {
        public string Server { get; private set; }

        private PingAverages _pingAverages = new PingAverages();
        private System.Threading.Timer _pingIPTimer;
        private int _pingErrors = 0;  

        public PingItem(string serverName, string ipToPing, double xPos, double yPos) : base()
        {
            Server = serverName;

            var config = ConfigManager.GetPingConfig();

            this.FontWeight = FontWeights.UltraBold;
            this.FontSize = config.PingFontSize;
            this.IsHitTestVisible = false;
    
            if (config.GreyBackground)
            {
                this.Background = Brushes.LightGray;
            }
            if (config.PingOutline)
            {
                this.Effect = new DropShadowEffect() { ShadowDepth = 0, BlurRadius = 5, Color = Colors.White, Opacity = 1 };
            }

            XPos = xPos;
            YPos = yPos;

            _pingIPTimer = new System.Threading.Timer(async (state) =>
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    try
                    {
                        var pReply = await ping.SendPingAsync(ipToPing);
                        if (pReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        {
                            _pingAverages.Add((int)pReply.RoundtripTime);
                            _pingErrors = 0;
                        }
                        else if (pReply.Status == System.Net.NetworkInformation.IPStatus.TimedOut)
                        {
                            _pingErrors++;
                        }

                        // If we fail 3 pings in a row, set the ping to "ERROR"
                    }
                    catch
                    {
                        _pingErrors++;
                    }
                }

                this.Dispatcher.Invoke(() =>
                {
                    if (_pingErrors == 3)
                    {
                        this.Text = $"{serverName}: ERROR";
                    }
                    else
                    {
                        if (_pingAverages.AveragePing >= 150) this.Foreground = config.HighPingBrush;
                        else if (_pingAverages.AveragePing >= 80) this.Foreground = config.MediumPingBrush;
                        else this.Foreground = config.LowPingBrush;

                        this.Text = $"{serverName}: {_pingAverages.AveragePing}";
                    }
                });
            }, null, 0, 1000);
        }

        public void MoveTo(double x, double y)
        {
            this.XPos = x;
            this.YPos = y;

            Canvas.SetLeft(this, XPos);
            Canvas.SetTop(this, YPos);
        }

        public bool IsPointOver(int x, int y)
        {
            return (x >= XPos && x <= (XPos + this.ActualWidth))
                && (y >= YPos && y <= (YPos + this.ActualHeight));
        }
    }
}
