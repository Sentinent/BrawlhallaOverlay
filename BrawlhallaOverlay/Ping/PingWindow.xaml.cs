using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace BrawlhallaOverlay.Ping
{
    /// <summary>
    /// Interaction logic for PingWindow.xaml
    /// </summary>
    public partial class PingWindow : Window
    {
        public Overlay.Overlay Overlay;
        public OptionsWindow OptionsWindow;

        public ColorWindow ColorSelector;
        private CancelEventHandler _preventColorClose;

        private Config _config = PingConfig.GetConfig();

        public PingWindow()
        {
            InitializeComponent();
        }

        private void PingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Overlay = new Overlay.Overlay();
            if (_config.OverlayEnabled)
            {
                Overlay.Show();
            }

            ColorSelector = new ColorWindow();
            ColorSelector.Owner = this;

            _preventColorClose = (send, even) =>
            {
                //OptionsWindow.ColorSelected((ColorSelector.Content as ColorPicker).SelectedColor);
                ColorSelector.Visibility = Visibility.Hidden; // Instead of closing the window, just hide it so we don't have to create multiple instances of it
                even.Cancel = true;
            };
            ColorSelector.Closing += _preventColorClose;
        }

        private void PingWindow_Closing(object sender, CancelEventArgs e)
        {
            OptionsWindow?.Close();
            Overlay?.Close();
            // we need to remove the event handling preventing the colorselector from closing
            ColorSelector.Closing -= _preventColorClose;
            ColorSelector?.Close();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadPingItems();
        }

        private void ReloadPingItems()
        {
            var panel = this.Content as StackPanel;
            panel.Children.Clear();

            var config = PingConfig.GetConfig();
            foreach (var server in config.ServersEnabled)
            {
                var item = new PingWindowItem(server.Name);
                panel.Children.Add(item.Panel);
            }

            var refreshAllButton = new Button()
            {
                Content = "Refresh All",
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5, 2, 5, 1)
            };
            refreshAllButton.Click += (_, __) =>
            {
                foreach (var control in panel.Children)
                {
                    var sPanel = control as StackPanel;
                    if (sPanel != null)
                    {
                        foreach (var child in sPanel.Children)
                        {
                            var button = child as Button;
                            if (button != null)
                            {
                                button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            }
                        }
                    }
                }
            };
            panel.Children.Add(refreshAllButton);

            var settingsButton = new Button()
            {
                Content = "Settings",
                Height = 25,
                Margin = new Thickness(5, 1, 5, 2)
            };
            settingsButton.Click += (send, even) =>
            {
                if (OptionsWindow == null) // null after closing
                {
                    OptionsWindow = new OptionsWindow();
                    OptionsWindow.Closed += (_, __) =>
                    {
                        ReloadPingItems();
                    };
                    OptionsWindow.Closed += (_, __) =>
                    {
                        OptionsWindow = null;
                    };
                }
                OptionsWindow.Owner = this;
                OptionsWindow.Show();              
            };
            panel.Children.Add(settingsButton);

            this.Height = 35 + (panel.Children.Count * 30);
        }
    }
}
