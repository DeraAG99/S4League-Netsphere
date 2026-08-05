using System.Windows.Forms;

namespace BlubLib.GUI.Controls
{
    public sealed class ViewHost : ContainerControl
    {
        private View _view;

        public View View
        {
            get { return _view; }
            set
            {
                if (_view == value)
                    return;

                var oldPage = _view;
                _view = value;
                Controls.Clear();
                if (_view == null)
                {
                    oldPage.OnDeactivateInternal();
                    oldPage.ViewHost = null;
                    return;
                }

                Controls.Add(_view);
                //_view.Dock = DockStyle.Fill;
                //_view.BorderStyle = BorderStyle.None;
                _view.ViewHost = this;
                _view.OnActivateInternal();
            }
        }

        public ViewHost()
        {
            AutoScroll = true;
        }
    }
}
