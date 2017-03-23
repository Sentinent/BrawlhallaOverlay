using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BrawlhallaOverlay.Overlay
{
    public class OverlayItem : TextBox
    {
        public double XPos { get; protected set; }
        public double YPos { get; protected set; }

        public OverlayItem()
        {
            this.Background = Brushes.Transparent;
            this.BorderThickness = new Thickness(0);
        }
    }
}
