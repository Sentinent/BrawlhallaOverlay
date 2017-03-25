using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace BrawlhallaOverlay.Overlay
{
    public class OverlayItem : TextBox
    {
        public double XPos { get; protected set; }
        public double YPos { get; protected set; }

        public string Identifier { get; protected set; }

        public OverlayItem()
        {
            this.Background = Brushes.Transparent;
            this.BorderThickness = new Thickness(0);
            this.FontWeight = FontWeights.UltraBold;
            this.IsHitTestVisible = false;
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
