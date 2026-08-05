using System;
using System.Runtime.InteropServices;

namespace BlubLib.WinAPI
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LVGROUP
    {
        public uint cbSize;
        public ListViewGroupFlag mask;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszHeader;
        public int cchHeader;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszFooter;
        public int cchFooter;
        public int iGroupId;
        public uint stateMask;
        public ListViewGroupState state;
        public uint uAlign;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszSubtitle;
        public uint cchSubtitle;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszTask;
        public uint cchTask;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszDescriptionTop;
        public uint cchDescriptionTop;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszDescriptionBottom;
        public uint cchDescriptionBottom;
        public int iTitleImage;
        public int iExtendedImage;
        public int iFirstItem;
        public uint cItems;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszSubsetTitle;
        public uint cchSubsetTitle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MENUINFO
    {
        public uint cbSize = (uint)Marshal.SizeOf(typeof(MENUINFO));
        public MenuInfoMask fMask = MenuInfoMask.Style;
        public MenuStyle dwStyle = MenuStyle.CheckOrBmp;
        public uint cyMax;
        public IntPtr hbrBack = IntPtr.Zero;
        public uint dwContextHelpID;
        public IntPtr dwMenuData = IntPtr.Zero;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    // ReSharper disable once InconsistentNaming
    public class MENUITEMINFO_T_RW
    {
        public uint cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO_T_RW));
        public MenuItemInfoMask fMask = MenuItemInfoMask.Bitmap;
        public uint fType;
        public uint fState;
        public uint wID;
        public IntPtr hSubMenu = IntPtr.Zero;
        public IntPtr hbmpChecked = IntPtr.Zero;
        public IntPtr hbmpUnchecked = IntPtr.Zero;
        public IntPtr dwItemData = IntPtr.Zero;
        public IntPtr dwTypeData = IntPtr.Zero;
        public uint cch;
        public IntPtr hbmpItem = IntPtr.Zero;
    }

    public delegate int BrowseForFolderCallback(IntPtr hwnd, BrowseForFolderMessage uMsg, IntPtr lParam, IntPtr lpData);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct BROWSEINFO
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public IntPtr pszDisplayName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszTitle;
        public BrowseInfoFlags ulFlags;
        public BrowseForFolderCallback lpfn;
        public IntPtr lParam;
        public int iImage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DwmBlurBehind
    {
        public DwmBlurBehindFlags dwFlags;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fEnable;
        public IntPtr hRgnBlur;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fTransitionOnMaximized;
    }
}
