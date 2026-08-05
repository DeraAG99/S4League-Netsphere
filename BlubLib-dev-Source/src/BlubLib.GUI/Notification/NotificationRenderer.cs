//using System.Drawing;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace BlubLib.GUI.Notification
//{
//    public class NotificationRenderer
//    {
//        private const float TitleXDiff = 10;
//        private const float TitleYDiff = 8;

//        private const float TextXDiff = 10;
//        private const float TextYDiff = 5;

//        public virtual void Render(Graphics graphics, NotificationWindow window)
//        {
//            using (var p = new Pen(Color.Black))
//                graphics.DrawRectangle(p, 0, 0, window.Width - 2, window.Height - 2);
//            using (var p = new Pen(Color.FromArgb(62, 67, 76)))
//                graphics.DrawRectangle(p, 1, 1, window.Width - 4, window.Height - 4);

//            using (var brush = new SolidBrush(window.ForeColor))
//            {
//                var titleSize = graphics.MeasureString(window.Title, window.TitleFont);
//                graphics.DrawString(window.Title, window.TitleFont, brush, TitleXDiff, TitleYDiff);

//                var textY = TitleYDiff + titleSize.Height + TextYDiff;
//                var rect = new RectangleF(TextXDiff, textY, 
//                    window.Width - textY*2,
//                    window.Height - (titleSize.Height + TitleYDiff + TextYDiff*2));

//                graphics.DrawString(window.Text, window.Font, brush, rect);
//            }
//        }

//        public virtual Task FadeIn(NotificationWindow window)
//        {
//            return Task.Factory.StartNew(() => FadeInTask(window));
//        }

//        public virtual Task FadeOut(NotificationWindow window)
//        {
//            return Task.Factory.StartNew(() => FadeOutTask(window));
//        }

//        private void FadeInTask(NotificationWindow window)
//        {
//            const float step = 0.2f;
//            const int loopCount = 5;
//            const int delay = 50;
//            for (var i = 0; i < loopCount; i++)
//            {
//                window.Invoke(new MethodInvoker(() =>
//                {
//                    var tmp = window.Opacity + step;
//                    window.Opacity = tmp > 1.0 ? 1.0 : tmp;
//                }));
//                Utilities.TaskDelay(delay).Wait();
//            }
//        }

//        private void FadeOutTask(NotificationWindow window)
//        {
//            const float step = 0.2f;
//            const int loopCount = 5;
//            const int delay = 50;
//            for (var i = 0; i < loopCount; i++)
//            {
//                window.Invoke(new MethodInvoker(() =>
//                {
//                    var tmp = window.Opacity - step;
//                    window.Opacity = tmp < 0.0 ? 0.0 : tmp;
//                }));
//                Utilities.TaskDelay(delay).Wait();
//            }
//        }
//    }
//}
