using System;

namespace BlubLib.DotNetty.SimpleRmi
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RmiAttribute : Attribute
    { }
}