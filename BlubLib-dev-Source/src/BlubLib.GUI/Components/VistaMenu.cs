using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BlubLib.WinAPI;

namespace BlubLib.GUI.Components
{
    [ProvideProperty("Image", typeof(MenuItem))]
    public class VistaMenu : Component, IExtenderProvider, ISupportInitialize
    {
        private const int SeparatorHeight = 9;
        private const int BorderVertical = 4;
        private const int LeftMargin = 4;
        private const int RightMargin = 6;
        private const int ShortcutMargin = 20;
        private const int ArrowMargin = 12;
        private const int IconSize = 16;

        private static Font _menuBoldFont;

        private ContainerControl _ownerForm;
        private Container _components;
        private bool _isUsingKeyboardAccel;
        private bool _formHasBeenIntialized;

        private readonly Hashtable _properties = new Hashtable();
        private readonly Hashtable _menuParents = new Hashtable();
        private readonly MENUINFO _mnuInfo = new MENUINFO();

        public ContainerControl ContainerControl
        {
            get { return _ownerForm; }
            set { _ownerForm = value; }
        }
        public override ISite Site
        {
            set
            {
                // Runs at design time, ensures designer initializes ContainerControl
                base.Site = value;
                var service = value?.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (service == null) return;
                var rootComponent = service.RootComponent;
                ContainerControl = rootComponent as ContainerControl;
            }
        }

        public VistaMenu()
        {
            InitializeComponent();
        }
        public VistaMenu(IContainer container) : this()
        {
            container.Add(this);
        }
        public VistaMenu(ContainerControl parentControl) : this()
        {
            _ownerForm = parentControl;
        }

        [DefaultValue(null)]
        [Description("The Image for the MenuItem")]
        [Category("Appearance")]
        public Image GetImage(MenuItem mnuItem)
        {
            return EnsurePropertiesExists(mnuItem).Image;
        }

        [DefaultValue(null)]
        public void SetImage(MenuItem mnuItem, Image value)
        {
            var prop = EnsurePropertiesExists(mnuItem);

            prop.Image = value;

            if (!DesignMode && Utilities.OperatingSystem >= OperatingSystem.WinVista)
            {
                //Destroy old bitmap object
                if (prop.RenderBmpHbitmap != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(prop.RenderBmpHbitmap);
                    prop.RenderBmpHbitmap = IntPtr.Zero;
                }

                //if there's no Image, then just bail out
                if (value == null)
                    return;

                //convert to 32bppPArgb (the 'P' means The red, green, and blue components are premultiplied, according to the alpha component.)
                using (var renderBmp = new Bitmap(IconSize, IconSize, System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
                {
                    using (var g = Graphics.FromImage(renderBmp))
                        g.DrawImage(value, 0, 0, IconSize, IconSize);

                    prop.RenderBmpHbitmap = renderBmp.GetHbitmap(Color.FromArgb(0, 0, 0, 0));
                }

                if (_formHasBeenIntialized)
                    AddVistaMenuItem(mnuItem);
            }


            //for every Pre-Vista Windows, add the parent of the menu item to the list of parents
            if (!DesignMode && Utilities.OperatingSystem < OperatingSystem.WinVista && _formHasBeenIntialized)
            {
                AddPreVistaMenuItem(mnuItem);
            }
        }

        private void InitializeComponent()
        {
            _components = new Container();
        }

        private void ownerForm_ChangeUICues(object sender, UICuesEventArgs e)
        {
            _isUsingKeyboardAccel = e.ShowKeyboard;
        }

        private static void MenuItem_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            var font = ((MenuItem)sender).DefaultItem
                ? _menuBoldFont
                : SystemFonts.MenuFont;

            if (((MenuItem)sender).Text == "-")
                e.ItemHeight = SeparatorHeight;
            else
            {
                e.ItemHeight = ((SystemFonts.MenuFont.Height > IconSize) ? SystemFonts.MenuFont.Height : IconSize)
                               + BorderVertical;

                e.ItemWidth = LeftMargin + IconSize + RightMargin

                    //item text width
                              + TextRenderer.MeasureText(((MenuItem)sender).Text, font, Size.Empty, TextFormatFlags.SingleLine | TextFormatFlags.NoClipping).Width
                              + ShortcutMargin

                    //shortcut text width
                              + TextRenderer.MeasureText(ShortcutToString(((MenuItem)sender).Shortcut), font, Size.Empty, TextFormatFlags.SingleLine | TextFormatFlags.NoClipping).Width

                    //arrow width
                              + ((((MenuItem)sender).IsParent) ? ArrowMargin : 0);
            }
        }

        private void MenuItem_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.InterpolationMode = InterpolationMode.Low;

            var menuSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            e.Graphics.FillRectangle(menuSelected ? SystemBrushes.Highlight : SystemBrushes.Menu, e.Bounds);

            if (((MenuItem)sender).Text == "-")
            {
                //draw the separator
                var yCenter = e.Bounds.Top + (e.Bounds.Height / 2) - 1;

                e.Graphics.DrawLine(SystemPens.ControlDark, e.Bounds.Left + 1, yCenter, (e.Bounds.Left + e.Bounds.Width - 2), yCenter);
                e.Graphics.DrawLine(SystemPens.ControlLightLight, e.Bounds.Left + 1, yCenter + 1, (e.Bounds.Left + e.Bounds.Width - 2), yCenter + 1);
            }
            else //regular menu items
            {
                //draw the item text
                DrawText(sender, e, menuSelected);

                if (((MenuItem)sender).Checked)
                {
                    ControlPaint.DrawMenuGlyph(e.Graphics,
                        e.Bounds.Left + (LeftMargin + IconSize + RightMargin - SystemInformation.MenuCheckSize.Width) / 2,
                        e.Bounds.Top + (e.Bounds.Height - SystemInformation.MenuCheckSize.Height) / 2 + 1,
                        SystemInformation.MenuCheckSize.Width,
                        SystemInformation.MenuCheckSize.Height,
                        ((MenuItem)sender).RadioCheck ? MenuGlyph.Bullet : MenuGlyph.Checkmark,
                        menuSelected ? SystemColors.HighlightText : SystemColors.MenuText,
                        menuSelected ? SystemColors.Highlight : SystemColors.Menu);
                }
                else
                {
                    var drawImg = GetImage(((MenuItem)sender));

                    if (drawImg == null) return;
                    //draw the image
                    if (((MenuItem)sender).Enabled)
                        e.Graphics.DrawImage(drawImg, e.Bounds.Left + LeftMargin,
                            e.Bounds.Top + ((e.Bounds.Height - IconSize) / 2),
                            IconSize, IconSize);
                    else
                        ControlPaint.DrawImageDisabled(e.Graphics, drawImg,
                            e.Bounds.Left + LeftMargin,
                            e.Bounds.Top + ((e.Bounds.Height - IconSize) / 2),
                            SystemColors.Menu);
                }
            }
        }

        private void DrawText(object sender, DrawItemEventArgs e, bool isSelected)
        {
            var shortcutText = ShortcutToString(((MenuItem)sender).Shortcut);

            var yPos = e.Bounds.Top + (e.Bounds.Height - SystemFonts.MenuFont.Height) / 2;

            var font = ((MenuItem)sender).DefaultItem
                ? _menuBoldFont
                : SystemFonts.MenuFont;

            var textSize = TextRenderer.MeasureText(((MenuItem)sender).Text,
                font, Size.Empty, TextFormatFlags.SingleLine | TextFormatFlags.NoClipping);

            var textRect = new Rectangle(e.Bounds.Left + LeftMargin + IconSize + RightMargin, yPos,
                textSize.Width, textSize.Height);

            if (!((MenuItem)sender).Enabled && !isSelected) // disabled and not selected
            {
                textRect.Offset(1, 1);

                TextRenderer.DrawText(e.Graphics, ((MenuItem)sender).Text, font,
                    textRect,
                    SystemColors.ControlLightLight,
                    TextFormatFlags.SingleLine | (_isUsingKeyboardAccel ? 0 : TextFormatFlags.HidePrefix) | TextFormatFlags.NoClipping);

                textRect.Offset(-1, -1);
            }

            //Draw the menu item text
            TextRenderer.DrawText(e.Graphics, ((MenuItem)sender).Text, font,
                textRect,
                ((MenuItem)sender).Enabled ? (isSelected ? SystemColors.HighlightText : SystemColors.MenuText) : SystemColors.GrayText,
                TextFormatFlags.SingleLine | (_isUsingKeyboardAccel ? 0 : TextFormatFlags.HidePrefix) | TextFormatFlags.NoClipping);



            //Draw the shortcut text
            if (shortcutText == null) return;
            textSize = TextRenderer.MeasureText(shortcutText,
                font, Size.Empty, TextFormatFlags.SingleLine | TextFormatFlags.NoClipping);


            textRect = new Rectangle(e.Bounds.Width - textSize.Width - ArrowMargin, yPos, textSize.Width,
                textSize.Height);

            if (!((MenuItem)sender).Enabled && !isSelected) // disabled and not selected
            {
                textRect.Offset(1, 1);

                TextRenderer.DrawText(e.Graphics, shortcutText, font,
                    textRect,
                    SystemColors.ControlLightLight,
                    TextFormatFlags.SingleLine | (_isUsingKeyboardAccel ? 0 : TextFormatFlags.HidePrefix) | TextFormatFlags.NoClipping);

                textRect.Offset(-1, -1);
            }

            TextRenderer.DrawText(e.Graphics, shortcutText, font,
                textRect,
                ((MenuItem)sender).Enabled ? (isSelected ? SystemColors.HighlightText : SystemColors.MenuText) : SystemColors.GrayText,
                TextFormatFlags.SingleLine | TextFormatFlags.NoClipping);
        }

        private Properties EnsurePropertiesExists(MenuItem key)
        {
            var p = (Properties)_properties[key];

            if (p != null) return p;

            p = new Properties();
            _properties[key] = p;

            return p;
        }

        private void AddVistaMenuItem(MenuItem mnuItem)
        {
            //get the bitmap children of the parent
            if (_menuParents[mnuItem.Parent] != null) return;
            if (mnuItem.Parent.GetType() == typeof(ContextMenu))
                ((ContextMenu)mnuItem.Parent).Popup += MenuItem_Popup;
            else
                ((MenuItem)mnuItem.Parent).Popup += MenuItem_Popup;

            //intialize all the topmost menus to be of type "MNS_CHECKORBMP" (for Vista classic theme)
            User32.SetMenuInfo(new HandleRef(null, mnuItem.Parent.Handle), _mnuInfo);

            _menuParents[mnuItem.Parent] = true;
        }

        private void AddPreVistaMenuItem(MenuItem mnuItem)
        {
            if (_menuParents[mnuItem.Parent] != null) return;
            _menuParents[mnuItem.Parent] = true;

            if (!_formHasBeenIntialized) return;
            //add all the menu items with custom paint events
            foreach (MenuItem menu in mnuItem.Parent.MenuItems)
            {
                menu.DrawItem += MenuItem_DrawItem;
                menu.MeasureItem += MenuItem_MeasureItem;
                menu.OwnerDraw = true;
            }
        }

        private void MenuItem_Popup(object sender, EventArgs e)
        {
            var menuItemInfo = new MENUITEMINFO_T_RW();

            // get the menu items collection
            var mi = sender.GetType() == typeof(ContextMenu) ? ((ContextMenu)sender).MenuItems : ((MenuItem)sender).MenuItems;

            // we have to track the menuPosition ourselves
            // because MenuItem.Index is only correct when
            // all the menu items are visible.
            var miOn = 0;
            for (var i = 0; i < mi.Count; i++)
            {
                if (!mi[i].Visible) continue;
                var p = ((Properties)_properties[mi[i]]);

                if (p != null)
                {
                    menuItemInfo.hbmpItem = p.RenderBmpHbitmap;

                    //refresh the menu item where ((Menu)sender).Handle is the parent handle
                    User32.SetMenuItemInfo(new HandleRef(null, ((Menu)sender).Handle),
                        (uint)miOn,
                        true,
                        menuItemInfo);
                }

                miOn++;
            }
        }

        bool IExtenderProvider.CanExtend(object o)
        {
            var item = o as MenuItem;
            if (item != null)
            {
                // reject the menuitem if it's a top level element on a MainMenu bar
                if (item.Parent != null)
                    return item.Parent.GetType() != typeof(MainMenu);

                // parent is null - meaning it's a context menu
                return true;
            }

            return o is Form;
        }

        void ISupportInitialize.BeginInit() { }

        void ISupportInitialize.EndInit()
        {
            if (DesignMode) return;
            if (Utilities.OperatingSystem >= OperatingSystem.WinVista)
            {
                foreach (DictionaryEntry de in _properties)
                    AddVistaMenuItem((MenuItem)de.Key);
            }
            else // Pre-Vista menus
            {
                // Declare the fonts once:
                //    If the user changes the menu fonts while your program is
                //    running, it's tough luck for the user.
                //
                //    This keeps a cap on the memory by avoiding unnecessary Font object
                //    creation/destruction on every MenuItem .Measure() and .Draw()
                _menuBoldFont = new Font(SystemFonts.MenuFont, FontStyle.Bold);


                if (_ownerForm != null)
                    _ownerForm.ChangeUICues += ownerForm_ChangeUICues;

                foreach (DictionaryEntry de in _properties)
                {
                    AddPreVistaMenuItem((MenuItem)de.Key);
                }

                //add event handle for each menu item's measure & draw routines
                foreach (var mnuItem in from DictionaryEntry parent in _menuParents from MenuItem mnuItem in ((Menu)parent.Key).MenuItems select mnuItem)
                {
                    mnuItem.DrawItem += MenuItem_DrawItem;
                    mnuItem.MeasureItem += MenuItem_MeasureItem;
                    mnuItem.OwnerDraw = true;
                }
            }

            _formHasBeenIntialized = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //release all the HBitmap handles created
                foreach (var de in _properties.Cast<DictionaryEntry>().Where(de => ((Properties)de.Value).RenderBmpHbitmap != IntPtr.Zero))
                    Gdi32.DeleteObject(((Properties)de.Value).RenderBmpHbitmap);


                _components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private static string ShortcutToString(Shortcut shortcut)
        {
            if (shortcut == Shortcut.None) return null;
            var keys = (Keys)shortcut;
            return TypeDescriptor.GetConverter(keys.GetType()).ConvertToString(keys);
        }
    }

    internal class Properties
    {
        public Image Image;
        public IntPtr RenderBmpHbitmap = IntPtr.Zero;
    }
}
