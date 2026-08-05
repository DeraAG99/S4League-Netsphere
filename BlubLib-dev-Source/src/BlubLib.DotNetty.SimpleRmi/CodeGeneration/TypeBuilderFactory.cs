using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BlubLib.DotNetty.SimpleRmi.CodeGeneration
{
    internal static class TypeBuilderFactory
    {
        private static readonly AssemblyBuilder s_assemblyBuilder;
        private static readonly ModuleBuilder s_moduleBuilder;

        static TypeBuilderFactory()
        {
            const string name = "BlubLib.Network.SimpleRmi.RmiAssembly";
            s_assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name),
                AssemblyBuilderAccess.Run);
            s_moduleBuilder = s_assemblyBuilder.DefineDynamicModule($"{name}.dll");

            // When trying to get custom attributes via reflection on a method inside a dynamic generated class
            // the jit tries to search the assembly on disk??? (but works fine on types/properties)
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) => e.Name.StartsWith(name) ? s_assemblyBuilder : null;
        }

        public static TypeBuilder Create(string name)
        {
            return s_moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Class);
        }

        public static TypeBuilder Create(string name, Type parent)
        {
            return s_moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Class, parent);
        }
    }
}
