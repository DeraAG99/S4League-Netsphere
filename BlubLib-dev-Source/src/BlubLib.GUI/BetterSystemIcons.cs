using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;
using BlubLib.WinAPI;

namespace BlubLib.GUI
{
    public static class BetterSystemIcons
    {
        private static readonly Lazy<Func<IntPtr, Icon>> s_iconCreator = new Lazy<Func<IntPtr, Icon>>(GetIconConstructor);

        public static Icon Information(Size size)
        {
            return GetIcon(Icons.Information, size);
        }

        public static Icon Information(int width, int height)
        {
            return GetIcon(Icons.Information, width, height);
        }

        public static Icon Warning(Size size)
        {
            return GetIcon(Icons.Warning, size);
        }

        public static Icon Warning(int width, int height)
        {
            return GetIcon(Icons.Warning, width, height);
        }

        public static Icon Error(Size size)
        {
            return GetIcon(Icons.Error, size);
        }

        public static Icon Error(int width, int height)
        {
            return GetIcon(Icons.Error, width, height);
        }

        public static Icon Question(Size size)
        {
            return GetIcon(Icons.Question, size);
        }

        public static Icon Question(int width, int height)
        {
            return GetIcon(Icons.Question, width, height);
        }

        public static Icon Shield(Size size)
        {
            return GetIcon(Icons.Shield, size);
        }

        public static Icon Shield(int width, int height)
        {
            return GetIcon(Icons.Shield, width, height);
        }

        private static Icon GetIcon(Icons icon, Size size)
        {
            return GetIcon(icon, size.Width, size.Height);
        }

        private static Icon GetIcon(Icons icon, int width, int height)
        {
            var moduleHandle = Kernel32.GetModuleHandle("User32");
            var iconHandle = User32.LoadImage(moduleHandle, (IntPtr)icon, ImageType.Icon, width, height, 0);
            if (iconHandle == IntPtr.Zero)
                throw new Win32Exception();

            return s_iconCreator.Value(iconHandle);
        }

        private static Func<IntPtr, Icon> GetIconConstructor()
        {
            var constructorInfo = typeof(Icon)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new[] { typeof(IntPtr), typeof(bool) }, null);

            if (constructorInfo == null)
                throw new Exception("Unable to find Icon constructor");

            var handleParameter = Expression.Parameter(typeof(IntPtr));
            var ownershipParameter = Expression.Parameter(typeof(bool));

            var body = Expression.New(constructorInfo, handleParameter, ownershipParameter);
            var lambda = Expression.Lambda<Func<IntPtr, bool, Icon>>(body, handleParameter, ownershipParameter);
            var func = lambda.Compile();

            return handle => func(handle, true);
        }

        private enum Icons
        {
            Warning = 101,
            Question = 102,
            Error = 103,
            Information = 104,
            Shield = 106
        }
    }
}
