using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BlubLib.GUI.Controls.Extended;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace BlubLib.GUI.Controls
{
    [ToolboxBitmap(typeof(Label))]
    public class SeparatorLabel : LabelEx
    {
        private readonly Pen _pen;

        public override bool AutoSize => false;

        public SeparatorLabel()
        {
            TextAlign = ContentAlignment.MiddleLeft;
            base.AutoSize = false;

            _pen = new Pen(Color.FromArgb(180, 180, 180));

            if (Application.RenderWithVisualStyles &&
                VisualStyleRenderer.IsElementDefined(VisualStyles.TaskDialog.FootnoteSeparator))
                _pen = new Pen(VisualStyles.TaskDialog.FootnoteSeparatorRenderer.Value.GetColor(ColorProperty.EdgeLightColor));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var size = GetPreferredSize(Size);
            var y = Height / 2 + 1;
            e.Graphics.DrawLine(_pen, size.Width + 1, y, Width, y);
        }
    }
}
