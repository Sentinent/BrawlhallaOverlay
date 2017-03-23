using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrawlhallaOverlay.Hooks
{
    public static class LowLevelMouseHook
    {
        // Credit to https://blogs.msdn.microsoft.com/toub/2006/05/03/low-level-mouse-hook-in-c/

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT point;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public static EventHandler<MouseHookEventArgs> MouseDown = delegate { };
        public static EventHandler<MouseHookEventArgs> MouseMoved = delegate { };
        public static EventHandler<MouseHookEventArgs> MouseUp = delegate { };

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_MOUSE_LL = 14;
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                if ((MouseMessages)wParam == MouseMessages.WM_LBUTTONDOWN)
                {
                    MouseDown.Invoke(null, new MouseHookEventArgs(hookStruct.point.x, hookStruct.point.y));
                }
                else if ((MouseMessages)wParam == MouseMessages.WM_MOUSEMOVE)
                {
                    MouseMoved.Invoke(null, new MouseHookEventArgs(hookStruct.point.x, hookStruct.point.y));
                }
                else if ((MouseMessages)wParam == MouseMessages.WM_LBUTTONUP)
                {
                    MouseUp.Invoke(null, new MouseHookEventArgs(hookStruct.point.x, hookStruct.point.y));
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public static IntPtr Hook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    _hookID = SetWindowsHookEx(WH_MOUSE_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
                    return _hookID;
                }
            }
        }

        public static bool UnHook()
        {
            return UnhookWindowsHookEx(_hookID);
        }
    }
}
