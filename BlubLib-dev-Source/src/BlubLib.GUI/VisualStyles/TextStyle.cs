using System;
using System.Windows.Forms.VisualStyles;

namespace BlubLib.GUI.VisualStyles
{
    public static class TextStyle
    {
        private const string ClassName = nameof(TextStyle);

        public static VisualStyleElement MainInstruction { get; } = VisualStyleElement.CreateElement(ClassName, 1, 0);
        public static Lazy<VisualStyleRenderer> MainInstructionRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(MainInstruction));

        public static VisualStyleElement Instruction { get; } = VisualStyleElement.CreateElement(ClassName, 2, 0);
        public static Lazy<VisualStyleRenderer> InstructionRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(Instruction));

        public static VisualStyleElement BodyTitle { get; } = VisualStyleElement.CreateElement(ClassName, 3, 0);
        public static Lazy<VisualStyleRenderer> BodyTitleRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(BodyTitle));

        public static VisualStyleElement BodyText { get; } = VisualStyleElement.CreateElement(ClassName, 4, 0);
        public static Lazy<VisualStyleRenderer> BodyTextRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(BodyText));

        public static class HyperLinkText
        {
            public static VisualStyleElement Normal { get; } = VisualStyleElement.CreateElement(ClassName, 6, 1);
            public static Lazy<VisualStyleRenderer> NormalRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(Normal));

            public static VisualStyleElement Hot { get; } = VisualStyleElement.CreateElement(ClassName, 6, 2);
            public static Lazy<VisualStyleRenderer> HotRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(Hot));

            public static VisualStyleElement Pressed { get; } = VisualStyleElement.CreateElement(ClassName, 6, 3);
            public static Lazy<VisualStyleRenderer> PressedRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(Pressed));

            public static VisualStyleElement Disabled { get; } = VisualStyleElement.CreateElement(ClassName, 6, 4);
            public static Lazy<VisualStyleRenderer> DisabledRenderer { get; } = new Lazy<VisualStyleRenderer>(() => new VisualStyleRenderer(Disabled));
        }
    }
}
