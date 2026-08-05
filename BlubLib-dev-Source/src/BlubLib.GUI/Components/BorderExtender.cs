using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BlubLib.WinAPI;

namespace BlubLib.GUI.Components
{
    public sealed class BorderExtender : Component
    {
        private bool _enabled;
        private Form _owner;
        private Padding _margins;
        private Brush _designModeBrush;
        private Color _designModeColor;

        public Form Owner
        {
            get { return _owner; }
            set
            {
                if (_owner != null)
                {
                    _owner.MouseDown -= Form_OnMouseDown;
                    _owner.Shown -= Form_OnShown;
                    _owner.Paint -= Form_OnPaint;
                }
                _owner = value;
                _owner.MouseDown += Form_OnMouseDown;
                _owner.Shown += Form_OnShown;
                _owner.Paint += Form_OnPaint;
            }
        }
        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;
                var host = value?.GetService<IDesignerHost>();
                if (host != null)
                {
                    var form = host.RootComponent as Form;
                    Owner = form;
                }
            }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                ExtendFrameIntoClientArea();
            }
        }
        public bool Movable { get; set; }
        public Padding Margins
        {
            get { return _margins; }
            set
            {
                _margins = value;
                ExtendFrameIntoClientArea();
            }
        }
        public Color DesignModeColor
        {
            get { return _designModeColor; }
            set
            {
                _designModeColor = value;
                _designModeBrush?.Dispose();
                _designModeBrush = new SolidBrush(value);
            }
        }

        public BorderExtender()
        {
            Movable = true;
            DesignModeColor = Color.SkyBlue;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_designModeBrush != null)
                {
                    _designModeBrush.Dispose();
                    _designModeBrush = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Form Events

        private void Form_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!Movable ||
                (Margins != new Padding(-1) && GetClientArea().Contains(e.Location)))
                return;

            User32.ReleaseCapture();
            User32.SendMessage(Owner.Handle, WindowMessage.NonClientLButtonDown, HitTest.Caption, 0);
        }

        private void Form_OnShown(object sender, EventArgs e)
        {
            ExtendFrameIntoClientArea();
        }

        private void Form_OnPaint(object sender, PaintEventArgs e)
        {
            if (!Enabled || (!DesignMode && !IsCompositionEnabled()))
                return;

            var brush = DesignMode ? _designModeBrush : Brushes.Black;
            if (Margins == new Padding(-1))
            {
                e.Graphics.FillRectangle(brush, Owner.ClientRectangle);
            }
            else
            {
                using (var r = new Region(Owner.ClientRectangle))
                {
                    r.Exclude(GetClientArea());
                    e.Graphics.FillRegion(brush, r);
                }
            }
        }

        #endregion

        private void ExtendFrameIntoClientArea()
        {
            if (Owner == null)
                return;

            Owner.Invalidate();

            if (!IsCompositionEnabled() || !Enabled)
                return;

            var margin = new Margins
            {
                Left = Margins.Left,
                Right = Margins.Right,
                Top = Margins.Top,
                Bottom = Margins.Bottom
            };
            Dwm.DwmExtendFrameIntoClientArea(Owner.Handle, ref margin);
        }

        private Rectangle GetClientArea()
        {
            return new Rectangle(Owner.ClientRectangle.Left + Margins.Left,
                Owner.ClientRectangle.Top + Margins.Top,
                Owner.ClientRectangle.Width - Margins.Horizontal,
                Owner.ClientRectangle.Height - Margins.Vertical);
        }

        private static bool IsCompositionEnabled()
        {
            if (Utilities.OperatingSystem < OperatingSystem.WinVista)
                return false;

            bool isEnabled;
            var result = Dwm.DwmIsCompositionEnabled(out isEnabled);
            if (result != 0)
                Marshal.ThrowExceptionForHR(result);
            return isEnabled;
        }
    }
}
