using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace BlubLib.GUI.Controls
{
    public sealed class Separator : UserControl
    {
        private readonly Pen _pen;

        public LineDirection LineDirection { get; set; }

        public Separator()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            Height = 10;

            _pen = new Pen(Color.FromArgb(180, 180, 180));

            if (Application.RenderWithVisualStyles &&
                VisualStyleRenderer.IsElementDefined(VisualStyles.TaskDialog.FootnoteSeparator))
                _pen = new Pen(VisualStyles.TaskDialog.FootnoteSeparatorRenderer.Value.GetColor(ColorProperty.EdgeLightColor));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            switch (LineDirection)
            {
                case LineDirection.Horizontal:
                    var y = Height / 2;
                    e.Graphics.DrawLine(_pen, 0, y, Width, y);
                    break;

                case LineDirection.Vertical:
                    var x = Width / 2;
                    e.Graphics.DrawLine(_pen, x, 0, x, Height);
                    break;
            }
        }
    }

    public enum LineDirection
    {
        Horizontal,
        Vertical
    }
}