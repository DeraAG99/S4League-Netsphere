using System;

namespace BlubLib.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        Inherited = false, AllowMultiple = false)]
    public class BlubContractAttribute : Attribute
    {
        public Type SerializerType { get; set; }
        public object[] SerializerParameters { get; set; }

        public BlubContractAttribute()
        { }

        public BlubContractAttribute(Type serializerType, params object[] serializerParameters)
        {
            SerializerType = serializerType;
            SerializerParameters = serializerParameters;
        }
    }
}
