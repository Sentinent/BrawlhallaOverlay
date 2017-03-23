using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaOverlay.Ping
{
    public class PingWindowItem
    {
        public StackPanel Panel;

        private Label _serverName;
        private Label _serverPing;
        private Button _refreshPing;

        public PingWindowItem(string serverName)
        {
            Panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,               
            };

            _serverName = new Label()
            {
                Content = serverName,
                Width = 50,
                Margin = new Thickness(5, 2, 30, 2)             
            };
            _serverPing = new Label()
            {
                Content = 0,
                Width = 50,
                Margin = new Thickness(5, 2, 20, 2)
            };
            _refreshPing = new Button()
            {
                Content = "Refresh",
                Width = 70,
                Margin = new Thickness(0, 2, 5, 2),
            };
            _refreshPing.Click += async (sender, e) =>
            {
                _serverPing.Content = "Pinging...";
                using (var p = new System.Net.NetworkInformation.Ping())
                {
                    try
                    {
                        var pReply = await p.SendPingAsync(Utilities.GetIPToPingFromName(_serverName.Content.ToString()));
                        if (pReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        {
                            _serverPing.Content = pReply.RoundtripTime.ToString();
                        }
                        else
                        {
                            _serverPing.Content = "ERROR";
                        }
                    }
                    catch
                    {
                        _serverPing.Content = "ERROR";
                    }
                }
            };

            Panel.Children.Add(_serverName);
            Panel.Children.Add(_serverPing);
            Panel.Children.Add(_refreshPing);
        }
    }
}
