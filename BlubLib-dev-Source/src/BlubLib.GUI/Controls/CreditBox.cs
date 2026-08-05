using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BlubLib.GUI.Controls
{
    public sealed partial class CreditBox : UserControl
    {
        private string _module;
        private string _website;
        private string _author;

        private readonly Pen _borderPen;
        private readonly Color _gradientColor1;
        private readonly Color _gradientColor2;

        public string Module
        {
            get { return _module; }
            set
            {
                _module = value;
                _moduleLabel.Text = _module;
                UpdatePosition();
            }
        }
        public string Website
        {
            get { return _website; }
            set
            {
                _website = value;
                toolTip.SetToolTip(_websiteLabel, _website);
            }
        }
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                _websiteLabel.Text = _author;
                UpdatePosition();
            }
        }

        public CreditBox()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            _borderPen = new Pen(Color.FromArgb(215, 215, 215));
            _gradientColor1 = Color.FromArgb(246, 246, 246);
            _gradientColor2 = Color.FromArgb(236, 236, 236);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(ClientRectangle, _gradientColor1, _gradientColor2, 90.0f))
                e.Graphics.FillRectangle(brush, ClientRectangle);

            e.Graphics.DrawRectangle(_borderPen, 0, 0, Width - 1, Height - 1);
        }

        protected override void OnResize(EventArgs e)
        {
            UpdatePosition();
            base.OnResize(e);
        }

        private void websiteLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Website);
        }

        private void UpdatePosition()
        {
            _websiteLabel.Location = _moduleLabel.Width > _websiteLabel.Width ?
                new Point(_moduleLabel.Width + 10 - _websiteLabel.Width, _websiteLabel.Location.Y) :
                new Point(10, _websiteLabel.Location.Y);
        }
    }
}
