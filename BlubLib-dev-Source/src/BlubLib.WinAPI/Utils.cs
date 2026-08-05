using System;
using System.Drawing;

namespace BlubLib.WinAPI
{
    public static class Utils
    {
        public const int MAX_PATH = 260;

        public static int LoWord(int value)
        {
            return value << 16;
        }

        public static int MakeLong(short lowPart, short highPart)
        {
            return (int)((ushort)lowPart | (uint)(highPart << 16));
        }

        public static IntPtr GetTrayHandle()
        {
            var taskBarHandle = User32.FindWindow("Shell_TrayWnd", null);
            return taskBarHandle != IntPtr.Zero ?
                User32.FindWindowEx(taskBarHandle, IntPtr.Zero, "TrayNotifyWnd", IntPtr.Zero) :
                IntPtr.Zero;
        }

        public static Rectangle GetTrayRectangle()
        {
            Rectangle rect;
            User32.GetWindowRect(GetTrayHandle(), out rect);
            return rect;
        }
    }
}
