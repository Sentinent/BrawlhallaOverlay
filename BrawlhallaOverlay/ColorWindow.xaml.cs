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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace BrawlhallaOverlay
{
    /// <summary>
    /// Interaction logic for ColorWindow.xaml
    /// </summary>
    public partial class ColorWindow : Window
    {
        public ColorWindow()
        {
            InitializeComponent();
        }

        private void SelectedColor_Changed(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            var optionsWindow = (this.Owner as PingWindow).OptionsWindow;
            optionsWindow.ColorSelected((sender as ColorPicker).SelectedColor);
        }
    }
}
