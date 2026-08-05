using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using BlubLib.WinAPI;

namespace BlubLib.GUI.Controls.Extended
{
    [ToolboxBitmap(typeof(TextBox))]
    public class TextBoxEx : TextBox
    {
        private string _cue = "";
        private bool _showCueOnFocus;

        public string Cue
        {
            get { return _cue; }
            set
            {
                _cue = value;
                SetCue();
            }
        }
        public bool ShowCueOnFocus
        {
            get { return _showCueOnFocus; }
            set
            {
                _showCueOnFocus = value;
                SetCue();
            }
        }
        public bool NumericOnly { get; set; }
        public bool AllowDecimal { get; set; }
        public bool AllowLeadingSign { get; set; }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetCue();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (NumericOnly)
            {
                if (!char.IsDigit(e.KeyChar))
                    e.Handled = true;

                if (e.KeyChar == (char)Keys.Delete || e.KeyChar == (char)Keys.Back)
                    e.Handled = false;

                var numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
                var keyStr = e.KeyChar.ToString();
                if (AllowDecimal && keyStr == numberFormatInfo.NumberDecimalSeparator)
                    e.Handled = false;

                if (AllowLeadingSign && keyStr == numberFormatInfo.NegativeSign)
                    e.Handled = false;
            }
            base.OnKeyPress(e);
        }

        private void SetCue()
        {
            if (!Utilities.IsMono && Utilities.OperatingSystem >= OperatingSystem.WinVista && IsHandleCreated)
                User32.SendMessage(Handle, EditControlMessage.SetCueBanner, _showCueOnFocus, _cue);
        }
    }
}
