using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BrawlhallaOverlay
{
    public static class Utilities
    {
        public static string GetIPToPingFromName(string serverName)
        {
            switch (serverName)
            {
                case "US-W":
                    return "pingtest-cal.brawlhalla.com";
                case "US-E":
                    return "pingtest-atl.brawlhalla.com";
                case "EU":
                    return "pingtest-ams.brawlhalla.com";
                case "SEA":
                    return  "pingtest-sgp.brawlhalla.com";
                case "AUS":
                    return "pingtest-aus.brawlhalla.com";
                case "BRZ":
                    return "pingtest-brs.brawlhalla.com";
                default:
                    throw new ArgumentException("Invalid server name.");
            }
        }

        public static Color GetDefaultPingColor(string boundary)
        {
            switch (boundary.ToLower())
            {
                case "low ping":
                    return Brushes.Green.Color;
                case "medium ping":
                    return Brushes.Yellow.Color;
                case "high ping":
                    return Brushes.Red.Color;
                default:
                    throw new ArgumentException("Invalid ping boundary.");
            }
        }
    }
}
