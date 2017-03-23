using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaOverlay
{
    public class MouseHookEventArgs
    {
        public int MouseXPos { get; private set; }
        public int MouseYPos { get; private set; }

        public MouseHookEventArgs(int x, int y)
        {
            MouseXPos = x;
            MouseYPos = y;
        }
    }
}
