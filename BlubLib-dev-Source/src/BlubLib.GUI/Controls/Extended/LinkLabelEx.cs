using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BlubLib.GUI.VisualStyles;
using BlubLib.WinAPI;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace BlubLib.GUI.Controls.Extended
{
    [ToolboxBitmap(typeof(LinkLabel))]
    public sealed class LinkLabelEx : LinkLabel
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
        public Color Color { get; set; }
        public Color HoverColor { get; set; }

        public LinkLabelEx()
        {
            LinkBehavior = LinkBehavior.HoverUnderline;
            ImageAlign = ContentAlignment.MiddleLeft;
            TextAlign = ContentAlignment.MiddleRight;

            Font = new Font("Segoe UI", 9, FontStyle.Regular);
            Color = Color.FromArgb(0, 102, 204);
            ActiveLinkColor = HoverColor = Color.FromArgb(51, 153, 255);

            if (!Application.RenderWithVisualStyles)
                return;

            if (VisualStyleRenderer.IsElementDefined(TextStyle.HyperLinkText.Normal))
                LinkColor = Color = TextStyle.HyperLinkText.NormalRenderer.Value.GetColor(ColorProperty.TextColor);

            if (VisualStyleRenderer.IsElementDefined(TextStyle.HyperLinkText.Hot))
                HoverColor = TextStyle.HyperLinkText.HotRenderer.Value.GetColor(ColorProperty.TextColor);

            if (VisualStyleRenderer.IsElementDefined(TextStyle.HyperLinkText.Pressed))
                HoverColor = TextStyle.HyperLinkText.PressedRenderer.Value.GetColor(ColorProperty.TextColor);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WindowMessage.SetCursor &&
                !Utilities.IsMono && Utilities.OperatingSystem >= OperatingSystem.Win2000)
            {
                User32.SetCursor(User32.LoadCursor(0, WinAPI.Cursor.Hand));
                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            LinkColor = HoverColor;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            LinkColor = Color;
            base.OnMouseLeave(e);
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
