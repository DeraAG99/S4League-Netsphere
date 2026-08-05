using System;

namespace BlubLib.WinAPI
{
    public enum ImageType
    {
        Bitmap = 0,
        Icon = 1,
        Cursor = 2
    }

    public enum ThumbnailProgressState
    {
        NoProgress = 0,
        Indeterminate = 1,
        Normal = 2,
        Error = 4,
        Paused = 8
    }

    public enum ProgressBarState
    {
        Normal = 1,
        Error = 2,
        Paused = 3
    }

    public enum ProgressBarStyle
    {
        SmoothReverse = 0x10,
    }

    public enum ProgressBarMessage
    {
        SetState = 0x410,
    }

    public enum ExtendedWindowStyle
    {
        ToolWindow = 0x80,
    }

    public enum EditControl
    {
        LeftMargin = 1,
        RightMargin = 2
    }

    public enum EditControlMessage
    {
        SetMargins = 0xD3,
        SetCueBanner = 0x1501,
    }

    public enum ComboBoxControlMessage
    {
        SetCueBanner = 0x1703,
    }

    public enum UiState
    {
        Set = 1,
    }

    public enum UiStateFlag
    {
        HideFocus = 1,
    }

    public enum ListViewGroupFlag
    {
        State = 0x00000004,
        Footer = 0x00000002,
    }

    public enum ListViewControlMessage
    {
        SetGroupInfo = 0x1093,
    }

    public enum Cursor
    {
        Hand = 32649,
    }

    public enum MenuInfoMask
    {
        Style = 0x00000010,
    }

    public enum MenuStyle
    {
        CheckOrBmp = 0x0400000
    }

    public enum MenuItemInfoMask
    {
        Bitmap = 0x00000080
    }

    public enum WindowMessage
    {
        Paint = 0x0F,
        ChangeUiState = 0x0127,
        //SysCommand = 0x0112,
        SetCursor = 0x0020,
        LButtonUp = 0x0202,
        NonClientHitTest = 0x0084,
        NonClientLButtonDown = 0x00A1,
    }

    public enum HitTest
    {
        Error = -2,
        Transparent = -1,
        NoWhere = 0,
        Client = 1,
        Caption = 2,
        SystemMenu = 3,
        GrowBox = 4,
        Menu = 5,
        HScroll = 6,
        VScroll = 7,
        MinButton = 8,
        Zoom = 9,
        Left = 10,
        Right = 11,
        Top = 12,
        TopLeft = 13,
        TopRight = 14,
        Bottom = 15,
        BottomLeft = 16,
        BottomRight = 17,
        Border = 18,
        Close = 20,
        Help = 21
    }

    public enum ButtonStyle
    {
        CommandLink = 0x0000000E
    }

    public enum ButtonControlMessage
    {
        SetNote = 0x00001609,
        GetNote = 0x0000160A,
        GetNoteLength = 0x0000160B,
        SetShield = 0x0000160C,
    }

    [Flags]
    public enum ListViewGroupState
    {
        Normal = 0,
        Collapsed = 1,
        Hidden = 2,
        NoHeader = 4,
        Collapsible = 8,
        Focused = 16,
        Selected = 32,
        SubSeted = 64,
        SubSetLinkFocused = 128,
    }

    [Flags]
    public enum BrowseInfoFlags : uint
    {
        ReturnOnlyFsDirs = 0x00000001,
        DontGoBelowDomain = 0x00000002,


        StatusText = 0x00000004,
        ReturnFsAncestors = 0x00000008,
        EditBox = 0x00000010,
        Validate = 0x00000020,
        NewDialogStyle = 0x00000040,
        BrowseIncludeUrls = 0x00000080,
        UseNewUi = EditBox | NewDialogStyle,
        UaHint = 0x00000100,
        NoNewFolderButton = 0x00000200,
        NoTranslateTargets = 0x00000400,
        BrowseForComputer = 0x00001000,
        BrowseForPrinter = 0x00002000,
        BrowseIncludeFiles = 0x00004000,
        Shareable = 0x00008000,
        BrowseFileJunctions = 0x00010000
    }

    public enum BrowseForFolderMessage : uint
    {
        Initialized = 1,
        SelChanged = 2,
        ValidateFailedA = 3,
        ValidateFailedW = 4,
        IUnkown = 5,

        // Send
        SetStatusText = 0x464,
        EnableOk = 0x465,
        SetSelectionA = 0x466,
        SetSelectionW = 0x467
    }

    [Flags]
    public enum DwmBlurBehindFlags : uint
    {
        Enable = 0x00000001,
        BlurRegion = 0x00000002,
        TransitiononMaximized = 0x00000004,
    }
}
