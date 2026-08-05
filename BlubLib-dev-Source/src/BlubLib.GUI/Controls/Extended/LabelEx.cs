using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BlubLib.GUI.VisualStyles;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace BlubLib.GUI.Controls.Extended
{
    [ToolboxBitmap(typeof(Label))]
    public class LabelEx : Label
    {
        private LabelStyle _labelStyle;

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
        public LabelStyle LabelStyle
        {
            get { return _labelStyle; }
            set
            {
                _labelStyle = value;
                SetLabelStyle();
            }
        }

        public LabelEx()
        {
            ImageAlign = ContentAlignment.MiddleLeft;
            TextAlign = ContentAlignment.MiddleRight;
            LabelStyle = LabelStyle.BodyText;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var size = base.GetPreferredSize(proposedSize);
            if (Image != null)
                size = new Size(size.Width + 3 + Image.Width, size.Height + Math.Abs(size.Height - Image.Height));
            return size;
        }

        private void SetLabelStyle()
        {
            switch (LabelStyle)
            {
                case LabelStyle.BodyText:
                    ForeColor = Color.FromArgb(0, 0, 0);
                    Font = new Font("Segoe UI", 9, FontStyle.Regular);
                    break;

                case LabelStyle.BodyTitle:
                    ForeColor = Color.FromArgb(0, 0, 0);
                    Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    break;

                case LabelStyle.MainInstruction:
                    Font = new Font("Segoe UI", 12, FontStyle.Regular);
                    ForeColor = Color.FromArgb(0, 51, 153);
                    break;

                case LabelStyle.Instruction:
                    Font = new Font("Segoe UI", 9, FontStyle.Regular);
                    ForeColor = Color.FromArgb(0, 51, 153);
                    break;

                default:
                    return;
            }

            var renderer = GetVisualStyleRenderer();
            if (renderer != null)
                ForeColor = renderer.GetColor(ColorProperty.TextColor);
        }

        private VisualStyleRenderer GetVisualStyleRenderer()
        {
            if (!Application.RenderWithVisualStyles)
                return null;

            switch (LabelStyle)
            {
                case LabelStyle.BodyText:
                    if (VisualStyleRenderer.IsElementDefined(TextStyle.BodyText))
                        return TextStyle.BodyTextRenderer.Value;
                    break;

                case LabelStyle.BodyTitle:
                    if (VisualStyleRenderer.IsElementDefined(TextStyle.BodyTitle))
                        return TextStyle.BodyTitleRenderer.Value;
                    break;

                case LabelStyle.MainInstruction:
                    if (VisualStyleRenderer.IsElementDefined(TextStyle.MainInstruction))
                        return TextStyle.MainInstructionRenderer.Value;
                    break;

                case LabelStyle.Instruction:
                    if (VisualStyleRenderer.IsElementDefined(TextStyle.Instruction))
                        return TextStyle.InstructionRenderer.Value;
                    break;

                default:
                    throw new Exception($"Invalid LabelStyle {LabelStyle}");
            }

            return null;
        }
    }

    public enum LabelStyle
    {
        None,
        BodyText,
        BodyTitle,
        MainInstruction,
        Instruction,
    }
}
