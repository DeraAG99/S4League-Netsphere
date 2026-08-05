using System;
using System.Drawing;
using System.Windows.Forms;
using BlubLib.WinAPI;
using ProgressBarStyle = System.Windows.Forms.ProgressBarStyle;

namespace BlubLib.GUI.Controls.Extended
{
    [ToolboxBitmap(typeof(ProgressBar))]
    public class ProgressBarEx : ProgressBar
    {
        private ProgressBarState _state = ProgressBarState.Normal;
        private bool _showInTaskbar;

        public bool SmoothReverse { get; set; }
        public bool ShowInTaskbar
        {
            get { return _showInTaskbar; }
            set
            {
                _showInTaskbar = value;
                UpdateTaskbar();
            }
        }
        public ProgressBarState State
        {
            get { return _state; }
            set
            {
                _state = value;
                SetState();
            }
        }
        public new int Value
        {
            get { return base.Value; }
            set
            {
                base.Value = value;
                UpdateTaskbar();
            }
        }
        public new int Maximum
        {
            get { return base.Maximum; }
            set
            {
                base.Maximum = value;
                UpdateTaskbar();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cParams = base.CreateParams;
                if (SmoothReverse)
                    cParams.Style |= (int)WinAPI.ProgressBarStyle.SmoothReverse; // Smooth animation when decreasing the value
                return cParams;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if ((WindowMessage)m.Msg == WindowMessage.Paint)
                SetState();
            base.WndProc(ref m);
        }

        private void SetState()
        {
            if (!Utilities.IsMono && Utilities.OperatingSystem >= OperatingSystem.WinVista && IsHandleCreated)
                User32.SendMessage(Handle, ProgressBarMessage.SetState, State, 0);
        }

        private void UpdateTaskbar()
        {
            var form = FindForm();
            if (form == null)
                return;

            if (ShowInTaskbar)
            {
                if (Style == ProgressBarStyle.Marquee)
                {
                    form.SetProgressState(ThumbnailProgressState.Indeterminate);
                }
                else
                {
                    switch (State)
                    {
                        case ProgressBarState.Normal:
                            form.SetProgressState(ThumbnailProgressState.Normal);
                            break;

                        case ProgressBarState.Error:
                            form.SetProgressState(ThumbnailProgressState.Error);
                            break;

                        case ProgressBarState.Paused:
                            form.SetProgressState(ThumbnailProgressState.Paused);
                            break;
                    }
                    form.SetProgressValue(Value, Maximum);
                }
            }
            else
            {
                form.SetProgressState(ThumbnailProgressState.NoProgress);
            }
        }
    }
}
