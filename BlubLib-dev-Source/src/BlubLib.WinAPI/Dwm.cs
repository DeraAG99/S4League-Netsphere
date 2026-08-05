using System;
using System.Runtime.InteropServices;

namespace BlubLib.WinAPI
{
    public static class Dwm
    {
        private const string DllName = "Dwmapi.dll";

        [DllImport(DllName)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        [DllImport(DllName)]
        public static extern int DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] out bool pfEnabled);

        [DllImport(DllName)]
        public static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DwmBlurBehind pBlurBehind);
    }
}
