using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using BlubLib.WinAPI;

namespace BlubLib.GUI
{
    public sealed class FolderBrowserDialogEx : CommonDialog
    {
        // Root node of the tree view.
        private Environment.SpecialFolder _rootFolder;

        // Description text to show.
        private string _descriptionText;

        // Folder picked by the user.
        private string _selectedPath;

        // Show the 'New Folder' button?

        // set to True when selectedPath is set after the dialog box returns
        // set to False when selectedPath is set via the SelectedPath property.
        // Warning! Be careful about race conditions when touching this variable.
        // This variable is determining if the PathDiscovery security check should
        // be bypassed or not.
        private bool _selectedPathNeedsCheck;

        // Callback function for the folder browser dialog (see VSWhidbey 551866)
        private BrowseForFolderCallback _callback;

        public FolderBrowserDialogEx()
        {
            Reset();
        }

        public bool ShowNewFolderButton { get; set; }

        public bool ShowFiles { get; set; }

        public string SelectedPath
        {
            get
            {
                if (string.IsNullOrEmpty(_selectedPath))
                    return _selectedPath;

                if (_selectedPathNeedsCheck)
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, _selectedPath).Demand();
                }
                return _selectedPath;
            }
            set
            {
                _selectedPath = value ?? string.Empty;
                _selectedPathNeedsCheck = false;
            }
        }

        public Environment.SpecialFolder RootFolder
        {
            get { return _rootFolder; }
            set
            {
                // FXCop:
                // leaving in Enum.IsDefined because this Enum is likely to grow and we dont own it.
                if (!Enum.IsDefined(typeof(Environment.SpecialFolder), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Environment.SpecialFolder));
                _rootFolder = value;
            }
        }

        public string Description
        {
            get { return _descriptionText; }
            set { _descriptionText = value ?? string.Empty; }
        }

        public override void Reset()
        {
            _rootFolder = Environment.SpecialFolder.Desktop;
            _descriptionText = string.Empty;
            _selectedPath = string.Empty;
            _selectedPathNeedsCheck = false;
            ShowNewFolderButton = true;
        }

        protected override bool RunDialog(IntPtr hWndOwner)
        {
            var pidlRoot = IntPtr.Zero;
            var returnValue = false;

            Shell32.SHGetSpecialFolderLocation(hWndOwner, _rootFolder, ref pidlRoot);
            if (pidlRoot == IntPtr.Zero)
            {
                Shell32.SHGetSpecialFolderLocation(hWndOwner, Environment.SpecialFolder.Desktop, ref pidlRoot);
                if (pidlRoot == IntPtr.Zero)
                    throw new InvalidOperationException("Unable to find root folder");
            }

            var options = BrowseInfoFlags.UseNewUi;
            if (!ShowNewFolderButton)
                options |= BrowseInfoFlags.NoNewFolderButton;
            if(ShowFiles)
                options |= BrowseInfoFlags.BrowseIncludeFiles;

            // The SHBrowserForFolder dialog is OLE/COM based, and documented as only being safe to use under the STA
            // threading model if the BIF_NEWDIALOGSTYLE flag has been requested (which we always do in mergedOptions
            // above). So make sure OLE is initialized, and throw an exception if caller attempts to invoke dialog
            // under the MTA threading model (...dialog does appear under MTA, but is totally non-functional).
            if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != System.Threading.ApartmentState.STA)
                throw new System.Threading.ThreadStateException("Thread must be STA");

            var pidlRet = IntPtr.Zero;
            var pszSelectedPath = IntPtr.Zero;
            var displayName = IntPtr.Zero;

            try
            {
                // Construct a BROWSEINFO
                var bi = new BROWSEINFO();

                displayName = Marshal.AllocHGlobal(Utils.MAX_PATH * Marshal.SystemDefaultCharSize);
                pszSelectedPath = Marshal.AllocHGlobal((Utils.MAX_PATH + 1) * Marshal.SystemDefaultCharSize);
                _callback = FolderBrowserDialog_BrowseCallbackProc;

                bi.pidlRoot = pidlRoot;
                bi.hwndOwner = hWndOwner;
                bi.pszDisplayName = displayName;
                bi.lpszTitle = _descriptionText;
                bi.ulFlags = options;
                bi.lpfn = _callback;
                bi.lParam = IntPtr.Zero;
                bi.iImage = 0;

                // And show the dialog
                pidlRet = Shell32.SHBrowseForFolder(ref bi);

                if (pidlRet != IntPtr.Zero)
                {
                    // Then retrieve the path from the IDList
                    Shell32.SHGetPathFromIDList(pidlRet, pszSelectedPath);

                    // set the flag to True before selectedPath is set to
                    // assure security check and avoid bogus race condition
                    _selectedPathNeedsCheck = true;

                    // Convert to a string
                    _selectedPath = Marshal.PtrToStringAuto(pszSelectedPath);

                    returnValue = true;
                }
            }
            finally
            {
                Ole32.CoTaskMemFree(pidlRoot);
                if (pidlRet != IntPtr.Zero)
                    Ole32.CoTaskMemFree(pidlRet);

                // Then free all the stuff we've allocated or the SH API gave us
                if (pszSelectedPath != IntPtr.Zero)
                    Marshal.FreeHGlobal(pszSelectedPath);

                if (displayName != IntPtr.Zero)
                    Marshal.FreeHGlobal(displayName);

                _callback = null;
            }
            return returnValue;
        }

        private int FolderBrowserDialog_BrowseCallbackProc(IntPtr hwnd,
                                                           BrowseForFolderMessage msg,
                                                           IntPtr lParam,
                                                           IntPtr lpData)
        {
            switch (msg)
            {
                case BrowseForFolderMessage.Initialized:
                    // Indicates the browse dialog box has finished initializing. The lpData value is zero.
                    if (_selectedPath.Length != 0)
                    {
                        // Try to select the folder specified by selectedPath
                        User32.SendMessage(hwnd, BrowseForFolderMessage.SetSelectionW, 1, _selectedPath);
                    }
                    break;

                case BrowseForFolderMessage.SelChanged:
                    // Indicates the selection has changed. The lpData parameter points to the item identifier list for the newly selected item.
                    var selectedPidl = lParam;
                    if (selectedPidl != IntPtr.Zero)
                    {
                        var pszSelectedPath = Marshal.AllocHGlobal((Utils.MAX_PATH + 1) * Marshal.SystemDefaultCharSize);
                        // Try to retrieve the path from the IDList
                        var isFileSystemFolder = Shell32.SHGetPathFromIDList(selectedPidl, pszSelectedPath);
                        Marshal.FreeHGlobal(pszSelectedPath);
                        User32.SendMessage(hwnd, BrowseForFolderMessage.EnableOk, 0, isFileSystemFolder);
                    }
                    break;
            }
            return 0;
        }
    }
}
