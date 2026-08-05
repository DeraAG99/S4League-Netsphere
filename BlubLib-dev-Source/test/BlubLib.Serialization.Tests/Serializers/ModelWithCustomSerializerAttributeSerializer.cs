using System;
using System.IO;
using BlubLib.Serialization.Tests.Models;

namespace BlubLib.Serialization.Tests.Serializers
{
    public class ModelWithCustomSerializerAttributeSerializer : ISerializer<ModelWithCustomSerializerAttribute>
    {
        public bool CanHandle(Type type) => type == typeof(ModelWithCustomSerializerAttribute);

        public void Serialize(BinaryWriter writer, ModelWithCustomSerializerAttribute value)
        {
            writer.Write(value.A);
        }

        public ModelWithCustomSerializerAttribute Deserialize(BinaryReader reader)
        {
            var foo = new ModelWithCustomSerializerAttribute { A = reader.ReadByte() };
            return foo;
        }
    }
}