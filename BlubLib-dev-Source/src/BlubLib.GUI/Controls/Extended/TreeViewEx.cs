using System;
using System.Drawing;
using System.Windows.Forms;
using BlubLib.WinAPI;

namespace BlubLib.GUI.Controls.Extended
{
    [ToolboxBitmap(typeof(TreeView))]
    public sealed class TreeViewEx : TreeView
    {
        public TreeViewEx()
        {
            FullRowSelect = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!Utilities.IsMono && Utilities.OperatingSystem >= OperatingSystem.WinVista)
                UxTheme.SetWindowTheme(Handle, "explorer", null);
        }
    }
}
