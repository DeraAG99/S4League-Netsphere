using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BlubLib.GUI.VisualStyles;

namespace BlubLib.GUI.Controls.Extended
{
    [ToolboxBitmap(typeof(Panel))]
    public sealed class PanelEx : Panel
    {
        private readonly Pen _pen;

        public BorderAlign BorderAlign { get; set; }
        public new BorderStyle BorderStyle { get; private set; }

        public PanelEx()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            BorderStyle = BorderStyle.None;

            _pen = new Pen(Color.FromArgb(180, 180, 180));

            if (Application.RenderWithVisualStyles &&
                VisualStyleRenderer.IsElementDefined(TaskDialog.FootnoteSeparator))
                _pen = new Pen(TaskDialog.FootnoteSeparatorRenderer.Value.GetColor(ColorProperty.EdgeLightColor));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (BorderAlign == BorderAlign.Left || BorderAlign == BorderAlign.All)
                e.Graphics.DrawLine(_pen, 0, 0, 0, Height - 1);

            if (BorderAlign == BorderAlign.Right || BorderAlign == BorderAlign.All)
                e.Graphics.DrawLine(_pen, Width - 1, 0, Width - 1, Height - 1);

            if (BorderAlign == BorderAlign.Top || BorderAlign == BorderAlign.All)
                e.Graphics.DrawLine(_pen, 0, 0, Width - 1, 0);

            if (BorderAlign == BorderAlign.Bottom || BorderAlign == BorderAlign.All)
                e.Graphics.DrawLine(_pen, 0, Height - 1, Width - 1, Height - 1);
        }
    }

    public enum BorderAlign
    {
        Left,
        Right,
        Top,
        Bottom,
        All,
        None,
    }
}
