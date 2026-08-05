using System;
using System.Windows.Forms.VisualStyles;

namespace BlubLib.GUI.VisualStyles
{
    public static class TaskDialog
    {
        private const string ClassName = nameof(TextStyle);

        public static VisualStyleElement FootnoteSeparator { get; } = VisualStyleElement.CreateElement(ClassName, 17, 0);
        public static Lazy<VisualStyleRenderer> FootnoteSeparatorRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(FootnoteSeparator));
    }
}
