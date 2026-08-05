using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlubLib.GUI.Controls.Extended
{
    [ToolboxBitmap(typeof(CheckBox))]
    public sealed class CheckBoxEx : CheckBox
    {
        public new Image Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;
                if (!AutoSize) return;
                // Force size calculation
                AutoSize = false;
                AutoSize = true;
            }
        }

        public CheckBoxEx()
        {
            ImageAlign = ContentAlignment.MiddleLeft;
            TextAlign = ContentAlignment.MiddleRight;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var size = base.GetPreferredSize(proposedSize);
            if (Image != null)
                size = new Size(size.Width + 3 + Image.Width, size.Height + Math.Abs(size.Height - Image.Height));
            return size;
        }
    }
}
