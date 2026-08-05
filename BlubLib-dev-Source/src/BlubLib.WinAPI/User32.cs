using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace BlubLib.WinAPI
{
    public static class User32
    {
        private const string DllName = "User32.dll";

        #region SendMessage

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, bool lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, StringBuilder lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref LVGROUP lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, ProgressBarMessage msg, int wParam, int lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, ProgressBarMessage msg, ProgressBarState wParam, int lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, WindowMessage msg, int wParam, int lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, ComboBoxControlMessage msg, int wParam, string lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, ButtonControlMessage msg, int wParam, int lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, ButtonControlMessage msg, int wParam, bool lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, ButtonControlMessage msg, int wParam, string lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, ButtonControlMessage msg, ref int wParam, StringBuilder lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, ListViewControlMessage msg, int wParam, ref LVGROUP lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, EditControlMessage msg, int wParam, string lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, EditControlMessage msg, [MarshalAs(UnmanagedType.Bool)] bool wParam, string lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, EditControlMessage msg, EditControl wParam, int lParam);

        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, BrowseForFolderMessage msg, int wParam, [MarshalAs(UnmanagedType.Bool)] bool lParam);

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, BrowseForFolderMessage msg, int wParam, string lParam);
        [DllImport(DllName)]
        public static extern int SendMessage(IntPtr hWnd, WindowMessage msg, HitTest wParam, int lParam);

        #endregion

        [DllImport(DllName)]
        public static extern int LoadCursor(int hInstance, int lpCursorName);

        [DllImport(DllName)]
        public static extern int LoadCursor(int hInstance, Cursor lpCursorName);

        [DllImport(DllName)]
        public static extern int SetCursor(int hCursor);

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuInfo(HandleRef hMenu, MENUINFO lpcmi);

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuItemInfo(HandleRef hMenu, uint uItem, [MarshalAs(UnmanagedType.Bool)] bool fByPosition, MENUITEMINFO_T_RW lpmii);

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr lpszWindow);

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport(DllName)]
        public static extern IntPtr LoadImage(IntPtr hinst, IntPtr lpszName, ImageType uType, int cxDesired, int cyDesired, uint fuLoad);

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseCapture();
    }
}
