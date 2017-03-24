using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BrawlhallaOverlay.Hooks;
using BrawlhallaOverlay.Ping;

namespace BrawlhallaOverlay.Overlay
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        public List<PingItem> PingItems = new List<PingItem>();

        private bool _moving;
        private PingItem _selectedItem;
        private Point _relativeMousePos;

        private WindowHook _winHook;

        public Overlay()
        {
            InitializeComponent();
        }

        public void AddItem(PingItem item)
        {
            PingItems.Add(item);
            (this.Content as Canvas).Children.Add(item);
            item.MoveTo(item.XPos, item.YPos);
        }

        public void RemoveItem(PingItem item)
        {
            PingItems.Remove(item);

            foreach (PingItem child in (this.Content as Canvas).Children)
            {
                if (child.Server == item.Server)
                {
                    (this.Content as Canvas).Children.Remove(child);
                    break;
                }
            }
        }

        private void Overlay_Loaded(object sender, RoutedEventArgs e)
        {
            // Start topmost updater
            _winHook = new WindowHook();
            _winHook.BrawlhallaOpened += (_, __) => MessageBox.Show("bh opened");
            _winHook.WindowFocused += (_, __) => this.Topmost = true;
            _winHook.LostWindowFocus += (_, __) => this.Topmost = false;

            // Add ping items
            var config = ConfigManager.GetPingConfig();
            foreach (var server in config.ServersEnabled)
            {
                var item = new PingItem(server.Name, server.PingLocation, server.xPos, server.yPos);
                PingItems.Add(item);

                (this.Content as Canvas).Children.Add(item);
                item.MoveTo(item.XPos, item.YPos);
            }

            // Create low level mouse hook
            LowLevelMouseHook.Hook();

            // Handle moving of ping items
            LowLevelMouseHook.MouseDown += Overlay_MouseDown;
            LowLevelMouseHook.MouseMoved += Overlay_MouseMoved;
            LowLevelMouseHook.MouseUp += Overlay_MouseUp;
        }

        private void Overlay_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LowLevelMouseHook.UnHook();
        }

        private void Overlay_MouseDown(object sender, MouseHookEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                foreach (PingItem pingItem in (this.Content as Canvas).Children)
                {
                    if (pingItem.IsPointOver(e.MouseXPos, e.MouseYPos))
                    {
                        _moving = true;
                        _selectedItem = pingItem;
                        _relativeMousePos = Mouse.GetPosition(_selectedItem);
                        return; // Only want to move 1 at a time for overlapping elements
                    }
                }
            }
        }

        private void Overlay_MouseMoved(object sender, MouseHookEventArgs e)
        {
            if (_moving && _selectedItem != null)
            {
                _selectedItem.MoveTo(e.MouseXPos - _relativeMousePos.X, e.MouseYPos - _relativeMousePos.Y);
            }
        }

        private void Overlay_MouseUp(object sender, MouseHookEventArgs e)
        {
            _moving = false;
            _selectedItem = null;
            _relativeMousePos = default(Point);

            // Save settings
            var config = ConfigManager.GetPingConfig();
            if (config.OverlayEnabled) // Make sure the overlay is enabled so we don't end up clearing the list
            {
                config.ServersEnabled.Clear();

                foreach (var server in PingItems)
                {
                    config.ServersEnabled.Add(new Server(server.Server, server.XPos, server.YPos));
                }
            }

            ConfigManager.SaveConfig();
        }
    }
}
