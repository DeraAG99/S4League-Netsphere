using System;
using System.Windows.Forms;
using BlubLib.WinAPI;

namespace BlubLib.GUI.Controls.Extended
{
    public class ComboBoxEx : ComboBox
    {
        private string _cue;

        public virtual string Cue
        {
            get { return _cue ?? ""; }
            set
            {
                _cue = value ?? "";
                SetCue();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (SelectedIndex == -1)
                SetCue();
        }

        private void SetCue()
        {
            if (!Utilities.IsMono && Utilities.OperatingSystem >= OperatingSystem.WinVista && IsHandleCreated)
                User32.SendMessage(Handle, ComboBoxControlMessage.SetCueBanner, 0, Cue);
        }
    }
}
