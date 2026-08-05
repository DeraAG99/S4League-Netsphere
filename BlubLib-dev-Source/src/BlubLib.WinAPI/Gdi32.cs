using System;
using System.Runtime.InteropServices;

namespace BlubLib.WinAPI
{
    public static class Gdi32
    {
        private const string DllName = "Gdi32.dll";

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
