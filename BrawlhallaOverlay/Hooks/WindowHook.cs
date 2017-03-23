using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaOverlay.Hooks
{
    public class WindowHook
    {
        // Detects when active window changes, stolen from SO :))
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMargins);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public EventHandler BrawlhallaOpened = delegate { };
        public EventHandler WindowFocused = delegate { };
        public EventHandler LostWindowFocus = delegate { };

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private IntPtr _procHandle = IntPtr.Zero;
        private WinEventDelegate _dele;
        private IntPtr _hook;

        private string GetActiveWindowTitle()
        {
            StringBuilder buffer = new StringBuilder(256);

            if (GetWindowText(GetForegroundWindow(), buffer, 256) > 0)
            {
                return buffer.ToString();
            }
            return "";
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (GetActiveWindowTitle() == "Brawlhalla")
            {
                if (_procHandle == IntPtr.Zero || !_procHandle.Equals(hwnd))
                {
                    // new brawlhalla window
                    BrawlhallaOpened.Invoke(this, EventArgs.Empty);
                    _procHandle = hwnd;

                }
                WindowFocused.Invoke(this, EventArgs.Empty);
            }
            else
            {
                LostWindowFocus.Invoke(this, EventArgs.Empty);
            }
        }

        public WindowHook()
        {
            var proc = Process.GetProcessesByName("Brawlhalla").FirstOrDefault();
            if (proc != null)
            {
                _procHandle = proc.Handle;
            }

            _dele = new WinEventDelegate(WinEventProc);
            _hook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }
    }
}
