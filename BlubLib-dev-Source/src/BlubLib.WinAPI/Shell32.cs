using System;
using System.Runtime.InteropServices;

namespace BlubLib.WinAPI
{
    public static class Shell32
    {
        private const string DllName = "Shell32.dll";

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

        [DllImport(DllName)]
        public static extern int SHGetMalloc([Out, MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc);

        [DllImport(DllName)]
        public static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, Environment.SpecialFolder nFolder, ref IntPtr ppidl);

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);
    }
}
