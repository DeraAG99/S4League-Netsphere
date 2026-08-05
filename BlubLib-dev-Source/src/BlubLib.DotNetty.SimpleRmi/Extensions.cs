using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace BlubLib.DotNetty.SimpleRmi
{
    internal static class Extensions
    {
        public static IEnumerable<MethodInfo> GetMethodsFlattenHierarchy(this Type @this)
        {
            return GetMethodsFlattenHierarchy(@this, BindingFlags.Default);
        }

        public static IEnumerable<MethodInfo> GetMethodsFlattenHierarchy(this Type @this, BindingFlags flags)
        {
            IEnumerable<MethodInfo> methods = @this.GetMethods();
            return @this.GetInterfaces().Aggregate(methods, (current, interfaceType) => current.Concat(interfaceType.GetMethods()));
            //return GetMethodsFlattenHierarchyRecursive(@this, flags);
        }

        //private static IEnumerable<MethodInfo> GetMethodsFlattenHierarchyRecursive(Type type, BindingFlags flags)
        //{
        //    foreach (var method in type.GetMethods())
        //        yield return method;

        //    foreach (var interfaceType in type.GetInterfaces())
        //        foreach (var method in GetMethodsFlattenHierarchyRecursive(interfaceType, flags))
        //            yield return method;
        //}
    }

    public static class SimpleRmiExtensions
    {
        public static T GetProxy<T>(this IChannel @this)
            where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be an interface", nameof(T));

            var handler = @this.Pipeline.Get<SimpleRmiHandler>();
            if (handler == null)
                throw new ArgumentException($"Channel pipeline does not contain {nameof(SimpleRmiHandler)}");

            return @this.Pipeline.Get<SimpleRmiHandler>().GetProxy<T>(@this);
        }

        public static Task KeepAliveAsync(this IChannel @this)
        {
            return @this.WriteAndFlushAsync(new KeepAliveMessage());
        }
    }
}
