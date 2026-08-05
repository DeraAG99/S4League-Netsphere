using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BlubLib.WinAPI;
using BlubLib.WinAPI.COM;

namespace BlubLib.GUI
{
    public static class FormExtensions
    {
        private static readonly ITaskbarList3 s_taskbarList;

        static FormExtensions()
        {
            if (Utilities.OperatingSystem >= OperatingSystem.Win7)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                s_taskbarList = (ITaskbarList3)new CTaskbarList();
                s_taskbarList.HrInit();
            }
        }

        public static void SetProgressState(this Form @this, ThumbnailProgressState state)
        {
            if (s_taskbarList != null && @this.IsHandleCreated)
                s_taskbarList.SetProgressState(@this.Handle, state);
        }

        public static void SetProgressValue(this Form @this, ulong current, ulong maximum)
        {
            if (s_taskbarList != null && @this.IsHandleCreated)
                s_taskbarList.SetProgressValue(@this.Handle, current, maximum);
        }

        public static void SetProgressValue(this Form @this, int current, int maximum)
        {
            if (s_taskbarList != null && @this.IsHandleCreated)
                @this.SetProgressValue((ulong)current, (ulong)maximum);
        }
    }

    public static class ControlExtensions
    {
        public static void SetTextWithoutEvent(this TextBox @this, string text, EventHandler @event)
        {
            @this.TextChanged -= @event;
            @this.Text = text;
            @this.TextChanged += @event;
        }

        public static void SetCheckedWithoutEvent(this CheckBox @this, bool @checked, EventHandler @event)
        {
            @this.CheckedChanged -= @event;
            @this.Checked = @checked;
            @this.CheckedChanged += @event;
        }

        public static void SetIndexWithoutEvent(this ComboBox @this, int index, EventHandler @event)
        {
            @this.SelectedIndexChanged -= @event;
            @this.SelectedIndex = index;
            @this.SelectedIndexChanged += @event;
        }

        public static void InvokeIfRequired(this Control @this, Action action)
        {
            if (@this.InvokeRequired)
                @this.Invoke(action);
            else
                action();
        }
    }

    public static class ListViewExtensions
    {
        public static void SetGroupState(this ListView @this, ListViewGroupState state)
        {
            foreach (ListViewGroup group in @this.Groups)
                SetGroupState(group, state);
        }

        public static void SetGroupState(this ListViewGroup @this, ListViewGroupState state)
        {
            if (Utilities.OperatingSystem < OperatingSystem.WinVista ||
                @this?.ListView == null)
                return;

            if (@this.ListView.InvokeRequired)
            {
                @this.ListView.Invoke(new Action(() => SetGroupState(@this, state)));
                return;
            }

            var lvgroup = new LVGROUP();
            lvgroup.cbSize = (uint)Marshal.SizeOf(lvgroup);
            lvgroup.state = state;
            lvgroup.mask = ListViewGroupFlag.State;

            lvgroup.iGroupId = GetGroupId(@this) ?? @this.ListView.Groups.IndexOf(@this);
            User32.SendMessage(@this.ListView.Handle, ListViewControlMessage.SetGroupInfo, lvgroup.iGroupId, ref lvgroup);
            User32.SendMessage(@this.ListView.Handle, ListViewControlMessage.SetGroupInfo, lvgroup.iGroupId, ref lvgroup);
            @this.ListView.Refresh();
        }

        public static void SetGroupFooter(this ListViewGroup @this, string footerText)
        {
            if (Utilities.OperatingSystem < OperatingSystem.WinVista ||
                @this?.ListView == null)
                return;

            if (@this.ListView.InvokeRequired)
            {
                @this.ListView.Invoke(new Action(() => SetGroupFooter(@this, footerText)));
                return;
            }

            var lvgroup = new LVGROUP();
            lvgroup.cbSize = (uint)Marshal.SizeOf(lvgroup);
            lvgroup.pszFooter = footerText;
            lvgroup.mask = ListViewGroupFlag.Footer;
            lvgroup.iGroupId = GetGroupId(@this) ?? @this.ListView.Groups.IndexOf(@this);
            User32.SendMessage(@this.ListView.Handle, ListViewControlMessage.SetGroupInfo, lvgroup.iGroupId, ref lvgroup);
        }

        private static int? GetGroupId(ListViewGroup group)
        {
            // http://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/ListViewGroup.cs,b7524887ae17a622
            int? id = null;
            var propertyInfo = typeof(ListViewGroup).GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo == null)
                return null;

            var val = propertyInfo.GetValue(group, null);
            if (val != null)
                id = (int?)val;

            return id;
        }
    }
}
