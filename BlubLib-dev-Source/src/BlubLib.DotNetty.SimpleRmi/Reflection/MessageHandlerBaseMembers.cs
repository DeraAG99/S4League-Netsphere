using System.Reflection;
using BlubLib.Reflection;

namespace BlubLib.DotNetty.SimpleRmi.Reflection
{
    internal static class MessageHandlerBaseMembers
    {
        public static readonly MethodInfo Send = ReflectionHelper.GetMethod(() => MessageHandlerBase.Send(null, null));
        public static readonly MethodInfo HandleAsync = ReflectionHelper.GetMethod(() => MessageHandlerBase.HandleAsync(null, null));
        public static readonly MethodInfo HandleWithResponseAsync =
            typeof(MessageHandlerBase).GetMethod(nameof(MessageHandlerBase.HandleWithResponseAsync), BindingFlags.Public | BindingFlags.Static);
    }
}
