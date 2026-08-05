using System;

namespace BlubLib.Serialization
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class BlubMemberAttribute : Attribute
    {
        public int Order { get; set; }
        public Type SerializerType { get; set; }
        public object[] SerializerParameters { get; set; }

        public BlubMemberAttribute(int order)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof(order));

            Order = order;
        }

        public BlubMemberAttribute(int order, Type serializerType, params object[] serializerParameters)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof(order));

            Order = order;
            SerializerType = serializerType;
            SerializerParameters = serializerParameters;
        }
    }
}
