using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrawlhallaOverlay.Ping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private Button _selectedPingColorButton;
        private Config _config = PingConfig.GetConfig();

        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void OptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OptionsWindow_Closing(object sender, CancelEventArgs e)
        {
            int fontSize;
            if (!Int32.TryParse(FontSizeTextBox.Text, out fontSize))
            {
                MessageBox.Show("Invalid font size!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Cancel = true;
                return;
            }
            _config.PingFontSize = fontSize;

            // Need to update existing pingitems with the new configs
            foreach (var item in (this.Owner as PingWindow).Overlay.PingItems)
            {
                item.FontSize = _config.PingFontSize;
            }

            PingConfig.SaveConfig();
        }

        // Servers Enabled

        private void ServersEnabled_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the server enabled states from config
            foreach (Button button in (sender as StackPanel).Children)
            {
                if (_config.ServersEnabled.Any(x => x.Name == button.Content.ToString()))
                {
                    button.Foreground = Brushes.Green;
                }
                else // servers are disabled by default
                {
                    button.Foreground = Brushes.Red;
                }
            }      
        }

        // User clicks on a server to toggle them
        private void ServerButton_Clicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var overlay = (this.Owner as PingWindow).Overlay;
            var server = _config.ServersEnabled.FirstOrDefault(x => x.Name == button.Content.ToString());
            if (server != null) // server was previously enabled, so we want to disable it
            {
                _config.ServersEnabled.Remove(server);
                button.Foreground = Brushes.Red;

                if (_config.OverlayEnabled)
                {
                    var item = overlay.PingItems.FirstOrDefault(x => x.Server == button.Content.ToString());
                    if (item != null)
                    {
                        overlay.RemoveItem(item);
                    }
                }
            }
            else // server was disabled, so we want to enable it
            {
                _config.ServersEnabled.Add(new Server(button.Content.ToString(), 0, 0));
                button.Foreground = Brushes.Green;

                if (_config.OverlayEnabled)
                {
                    overlay.AddItem(new PingItem(button.Content.ToString(), Utilities.GetIPToPingFromName(button.Content.ToString()), 0, 0));
                }
            }
        }


        // Overlay Settings

        private void OverlaySettings_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up colors from config
            if (_config.OverlayEnabled)
            {
                OverlayEnabledButton.Foreground = Brushes.Green;
            }
            else
            {
                OverlayEnabledButton.Foreground = Brushes.Red;
            }

            if (_config.GreyBackground)
            {
                OverlayBackgroundButton.Foreground = Brushes.Green;
            }
            else
            {
                OverlayBackgroundButton.Foreground = Brushes.Red;
            }

            if (_config.PingOutline)
            {
                OverlayOutlineButton.Foreground = Brushes.Green;
            }
            else
            {
                OverlayOutlineButton.Foreground = Brushes.Red;
            }

            FontSizeTextBox.Text = _config.PingFontSize.ToString();

            LowPingButton.Foreground = _config.LowPingBrush;
            MediumPingButton.Foreground = _config.MediumPingBrush;
            HighPingButton.Foreground = _config.HighPingBrush;
        }

        // Toggling overlay on and off
        private void OverlayEnabled_Clicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            _config.OverlayEnabled = !_config.OverlayEnabled;
            if (_config.OverlayEnabled)
            {
                button.Foreground = Brushes.Green;
            }
            else
            {
                button.Foreground = Brushes.Red;
            }

            // Hide or show overlay
            var overlay = (this.Owner as PingWindow).Overlay;
            if (_config.OverlayEnabled)
            {
                overlay.Show();
            }
            else
            {
                overlay.Hide();
            }
        }

        // When the grey background button is clicked
        private void GreyBackground_Clicked(object sender, RoutedEventArgs e)
        {
            _config.GreyBackground = !_config.GreyBackground;
            if (_config.GreyBackground)
            {
                OverlayBackgroundButton.Foreground = Brushes.Green;
            }
            else
            {
                OverlayBackgroundButton.Foreground = Brushes.Red;
            }

            foreach (var item in (this.Owner as PingWindow).Overlay.PingItems)
            {
                if (_config.GreyBackground)
                {
                    item.Background = Brushes.LightGray;
                }
                else
                {
                    item.Background = Brushes.Transparent;
                }
            }
        }

        private void PingOutline_Clicked(object sender, RoutedEventArgs e)
        {
            _config.PingOutline = !_config.PingOutline;
            if (_config.PingOutline)
            {
                OverlayOutlineButton.Foreground = Brushes.Green;
            }
            else
            {
                OverlayOutlineButton.Foreground = Brushes.Red;
            }

            foreach (var item in (this.Owner as PingWindow).Overlay.PingItems)
            {          
                if (_config.PingOutline)
                {
                    item.Effect = new DropShadowEffect() { ShadowDepth = 0, BlurRadius = 5, Color = Colors.White, Opacity = 1 };
                }
                else
                {
                    item.ClearValue(EffectProperty);
                }
            }
        }

        // Called when one of "Low Ping", "Medium Ping", or "High Ping" is clicked
        private void PingColor_Clicked(object sender, RoutedEventArgs e)
        {
            _selectedPingColorButton = sender as Button;

            var colorPicker = (this.Owner as PingWindow).ColorSelector; // get the colorwindow from the owner because we recreate instances of optionswindow
            colorPicker.Visibility = Visibility.Visible;
        }

        public void ColorSelected(Color? selectedColor)
        {
            Color color;
            if (selectedColor.HasValue)
            {
                color = selectedColor.Value;
            }
            else
            {
                color = PingConfig.GetDefaultColor(_selectedPingColorButton.Content.ToString());
            }

            switch (_selectedPingColorButton.Content)
            {
                case "Low Ping":
                    _config.LowPingColor = color;
                    _selectedPingColorButton.Foreground = _config.LowPingBrush;
                    break;
                case "Medium Ping":
                    _config.MediumPingColor = color;
                    _selectedPingColorButton.Foreground = _config.MediumPingBrush;
                    break;
                case "High Ping":
                    _config.HighPingColor = color;
                    _selectedPingColorButton.Foreground = _config.HighPingBrush;
                    break;               
                default:
                    throw new ArgumentException("Invalid ping button name.");
            }
            
        }
    }
}
