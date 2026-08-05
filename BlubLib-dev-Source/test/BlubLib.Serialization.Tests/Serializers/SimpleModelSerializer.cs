using System;
using System.IO;
using BlubLib.Serialization.Tests.Models;

namespace BlubLib.Serialization.Tests.Serializers
{
    public class SimpleModelSerializer : ISerializer<SimpleModel>
    {
        public bool CanHandle(Type type) => type == typeof(SimpleModel);

        public void Serialize(BinaryWriter writer, SimpleModel value)
        {
            writer.Write((int)value.A);
        }

        public SimpleModel Deserialize(BinaryReader reader)
        {
            return new SimpleModel
            {
                A = (byte)reader.ReadInt32()
            };
        }
    }
}
