using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BlubLib.GUI.Controls.Extended;
using BlubLib.GUI.Properties;
using BlubLib.WinAPI;

namespace BlubLib.GUI.Controls
{
    [ToolboxBitmap(typeof(TextBox))]
    public class SearchBox : TextBoxEx
    {
        private const int ButtonWidth = 26;
        private readonly SearchBoxButton _button;
        private readonly SearchBoxButton _alternateButton;
        private bool _alternateButtonEnabled;

        public bool AlternateButtonEnabled
        {
            get { return _alternateButtonEnabled; }
            set
            {
                _alternateButtonEnabled = value;
                UpdateButtons();
            }
        }

        public Image Image
        {
            get { return _button.Image; }
            set { _button.Image = value ?? Resources.magnifier; }
        }
        public Image AlternateImage
        {
            get { return _alternateButton.Image; }
            set { _alternateButton.Image = value ?? Resources.cross_small; }
        }
        public bool DrawState
        {
            get { return _button.DrawState; }
            set { _button.DrawState = value; }
        }
        public bool DrawAlternateState
        {
            get { return _alternateButton.DrawState; }
            set { _alternateButton.DrawState = value; }
        }

        public event EventHandler Action;
        public event EventHandler AlternateAction;

        protected virtual void OnAction()
        {
            Action?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAlternateAction()
        {
            AlternateAction?.Invoke(this, EventArgs.Empty);
            Clear();
        }

        public SearchBox()
        {
            _alternateButtonEnabled = true;
            _button = new SearchBoxButton
            {
                Visible = true,
                DrawState = false,
                Width = ButtonWidth,
                Image = Resources.magnifier
            };
            _button.Click += ButtonOnClick;

            _alternateButton = new SearchBoxButton
            {
                Visible = false,
                DrawState = true,
                Width = ButtonWidth,
                Image = Resources.cross_small
            };
            _alternateButton.Click += AlternateButtonOnClick;

            Controls.Add(_button);
            Controls.Add(_alternateButton);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!Utilities.IsMono && Utilities.OperatingSystem >= OperatingSystem.WinVista && IsHandleCreated)
                User32.SendMessage(Handle, EditControlMessage.SetMargins, EditControl.RightMargin, Utils.LoWord(ButtonWidth));
        }

        protected override void OnTextChanged(EventArgs e)
        {
            UpdateButtons();
            base.OnTextChanged(e);
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            OnAction();
        }

        private void AlternateButtonOnClick(object sender, EventArgs eventArgs)
        {
            OnAlternateAction();
        }

        private void UpdateButtons()
        {
            if (!AlternateButtonEnabled)
            {
                _button.Visible = true;
                _alternateButton.Visible = false;
            }
            else if (string.IsNullOrEmpty(Text))
            {
                _button.Visible = true;
                _alternateButton.Visible = false;
            }
            else
            {
                _button.Visible = false;
                _alternateButton.Visible = true;
            }
        }

        private sealed class SearchBoxButton : UserControl
        {
            private readonly VisualStyleRenderer _hoverRenderer;
            private readonly VisualStyleRenderer _pressedRenderer;
            private ButtonState _state;
            private Size _imageSize;
            private Image _image;

            public Image Image
            {
                get { return _image; }
                set
                {
                    _image = value;
                    Invalidate();
                }
            }
            public Size ImageSize
            {
                get { return _imageSize; }
                set
                {
                    _imageSize = value;
                    Invalidate();
                }
            }
            public bool DrawState { get; set; }

            public SearchBoxButton()
            {
                if (!Application.RenderWithVisualStyles ||
                    !VisualStyleRenderer.IsElementDefined(VisualStyleElement.Button.PushButton.Hot) ||
                    !VisualStyleRenderer.IsElementDefined(VisualStyleElement.Button.PushButton.Pressed))
                    throw new NotSupportedException("This control is not supported on your system");

                _hoverRenderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Hot);
                _pressedRenderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Pressed);
                DrawState = true;
                Dock = DockStyle.Right;
                Cursor = Cursors.Default;
                Width = 30;
                ImageSize = new Size(14, 14);
                SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

                BackgroundImageLayout = ImageLayout.Center;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (DrawState)
                {
                    switch (_state)
                    {
                        case ButtonState.Hover:
                            _hoverRenderer.DrawBackground(e.Graphics, ClientRectangle);
                            break;

                        case ButtonState.Pressed:
                            _pressedRenderer.DrawBackground(e.Graphics, ClientRectangle);
                            break;
                    }
                }

                var imageRect = new Rectangle(Width - Width / 2 - ImageSize.Width / 2, Height / 2 - ImageSize.Height / 2, ImageSize.Width, ImageSize.Height);
                e.Graphics.DrawImage(Image, imageRect);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                _state = ButtonState.None;
                Invalidate();
                base.OnMouseLeave(e);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (_state != ButtonState.Pressed)
                {
                    _state = ButtonState.Pressed;
                    Invalidate();
                }
                base.OnMouseDown(e);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                if (_state == ButtonState.Pressed)
                {
                    _state = ButtonState.None;
                    Invalidate();
                }
                base.OnMouseUp(e);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (_state != ButtonState.Pressed && _state != ButtonState.Hover)
                {
                    _state = ButtonState.Hover;
                    Invalidate();
                }
                base.OnMouseMove(e);
            }

            private enum ButtonState
            {
                None,
                Hover,
                Pressed
            }
        }

    }
}
