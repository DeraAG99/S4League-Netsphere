using System;
using System.Runtime.InteropServices;

namespace BlubLib.WinAPI
{
    public static class Ole32
    {
        private const string DllName = "Ole32.dll";

        [DllImport(DllName)]
        public static extern void CoTaskMemFree();

        [DllImport(DllName)]
        public static extern void CoTaskMemFree(IntPtr pv);
    }
}
