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

namespace BrawlhallaOverlay
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        #region Window Detection
        // Detects when active window changes, stolen from SO :))
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMargins);

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        WinEventDelegate dele = null;

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (GetActiveWindowTitle() == "Brawlhalla")
            {
                this.Topmost = true;
            }
            else
            {
                this.Topmost = false;
            }
        }
        #endregion

        public List<PingItem> PingItems = new List<PingItem>();

        private bool _moving;
        private PingItem _selectedItem;
        private Point _relativeMousePos;

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
                if (child.Name == item.Name)
                {
                    (this.Content as Canvas).Children.Remove(child);
                    break;
                }
            }
        }

        private void Overlay_Loaded(object sender, RoutedEventArgs e)
        {
            // Start topmost updater
            dele = new WinEventDelegate(WinEventProc);
            var hook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);

            // Add ping items
            var config = PingConfig.GetConfig();
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

            PingConfig.EditServersEnabled(PingItems);
            PingConfig.SaveConfig();
        }
    }
}
