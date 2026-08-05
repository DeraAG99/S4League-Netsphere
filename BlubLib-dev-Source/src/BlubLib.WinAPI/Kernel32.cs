using System;
using System.Runtime.InteropServices;

namespace BlubLib.WinAPI
{
    public static class Kernel32
    {
        private const string DllName = "Kernel32.dll";

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
