//using System;
//using System.Drawing;
//using System.Windows.Forms;
//using BlubLib.Native;

//namespace BlubLib.GUI.Notification
//{
//    public sealed class NotificationWindow : Form
//    {
//        private readonly NotificationRenderer _renderer;
//        private readonly int _duration;

//        public new FormBorderStyle FormBorderStyle
//        {
//            get { return base.FormBorderStyle; }
//            private set { base.FormBorderStyle = value; }
//        }

//        public Font TitleFont { get; set; }
//        public string Title { get; set; }

//        private NotificationWindow(int duration, NotificationRenderer renderer)
//        {
//            DoubleBuffered = true;
//            SetStyle(ControlStyles.UserPaint, true);
//            FormBorderStyle = FormBorderStyle.None;

//            _renderer = renderer;
//            _duration = duration;

//            StartPosition = FormStartPosition.Manual;
//            Size = new Size(300, 120);
//            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Size.Width - 8, Screen.PrimaryScreen.WorkingArea.Height - Size.Height - 8);
//            Opacity = 0;
//            TopMost = true;
//            ShowInTaskbar = false;
//            ShowIcon = false;

//            Font = new Font("Segoe UI", 9);
//            TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);

//            ForeColor = Color.White;
//            BackColor = Color.FromArgb(29, 38, 51);

//            Title = "NotificationWindow";
//            Text = "Notification";
//        }

//        protected override CreateParams CreateParams
//        {
//            get
//            {
//                var cp = base.CreateParams;
//                cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW;
//                return cp;
//            }
//        }

//        protected override void OnPaint(PaintEventArgs e)
//        {
//            e.Graphics.Clear(BackColor);
//            _renderer.Render(e.Graphics, this);
//        }

//        protected override void OnShown(EventArgs e)
//        {
//            Utilities.DelayedInvoke(new Action(() => _renderer.FadeIn(this).Wait()), 300);
//            Utilities.DelayedInvoke(new Action(() => _renderer.FadeOut(this).Wait()), _duration).ContinueWith(t => FadeOutCompleted());
//            base.OnShown(e);
//        }

//        private void FadeOutCompleted()
//        {
//            if (InvokeRequired)
//            {
//                Invoke(new Action(FadeOutCompleted));
//                return;
//            }
//            Close();
//        }

//        public static NotificationWindow Show(string title, string text, int duration, NotificationRenderer renderer)
//        {
//            var wnd = new NotificationWindow(duration, renderer)
//            {
//                Title = title, 
//                Text = text
//            };
//            wnd.Show();
//            return wnd;
//        }
//        public static NotificationWindow Show(string title, string text)
//        {
//            return Show(title, text, 5000, new NotificationRenderer());
//        }
//        public static NotificationWindow Show(string title, string text, int duration)
//        {
//            return Show(title, text, duration, new NotificationRenderer());
//        }
//        public static NotificationWindow Show(string title, string text, NotificationRenderer renderer)
//        {
//            return Show(title, text, 5000, renderer);
//        }
//    }
//}
