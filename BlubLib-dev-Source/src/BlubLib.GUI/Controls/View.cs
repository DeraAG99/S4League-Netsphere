using System;
using System.Windows.Forms;

namespace BlubLib.GUI.Controls
{
    public class View : UserControl
    {
        public event EventHandler Activate;
        public event EventHandler Deactivate;

        protected virtual void OnActivate()
        {
            Activate?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDeactivate()
        {
            Deactivate?.Invoke(this, EventArgs.Empty);
        }

        public ViewHost ViewHost { get; internal set; }

        protected override void OnParentChanged(EventArgs e)
        {
            if (!DesignMode && Parent != null && !(Parent is ViewHost))
                throw new InvalidOperationException("The ViewHost can only display Views");

            base.OnParentChanged(e);
        }

        internal void OnActivateInternal()
        {
            OnActivate();
        }

        internal void OnDeactivateInternal()
        {
            OnDeactivate();
        }
    }
}
