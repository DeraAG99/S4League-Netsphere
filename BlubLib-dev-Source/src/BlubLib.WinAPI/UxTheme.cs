using System;
using System.Runtime.InteropServices;

namespace BlubLib.WinAPI
{
    public static class UxTheme
    {
        private const string DllName = "UxTheme.dll";

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
    }
}
