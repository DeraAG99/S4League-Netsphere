using System;
using System.Drawing;
using System.Windows.Forms;
using BlubLib.WinAPI;

namespace BlubLib.GUI.Controls.Extended
{
    [ToolboxBitmap(typeof(ListView))]
    public sealed class ListViewEx : ListView
    {
        public ListViewEx()
        {
            FullRowSelect = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == (int)WindowMessage.LButtonUp)
                Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!Utilities.IsMono && Utilities.OperatingSystem >= OperatingSystem.WinVista)
                UxTheme.SetWindowTheme(Handle, "explorer", null);
        }
    }
}
