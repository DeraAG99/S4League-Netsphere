using System;
using BlubLib.Serialization.Tests.Models;
using Sigil;
using Sigil.NonGeneric;

namespace BlubLib.Serialization.Tests.Serializers
{
    public class ModelWithCustomCompilerAttributeCompiler : ISerializerCompiler
    {
        public bool CanHandle(Type type) => type == typeof(ModelWithCustomCompilerAttribute);

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.NewObject<ModelWithCustomCompilerAttribute>();
            emiter.StoreLocal(value);

            using (var a = emiter.DeclareLocal<byte>())
            {
                emiter.CallDeserializerForType(a.LocalType, a);

                emiter.LoadLocal(value);
                emiter.LoadLocal(a);
                emiter.Call(typeof(ModelWithCustomCompilerAttribute).GetProperty(nameof(ModelWithCustomCompilerAttribute.A)).SetMethod);
            }
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            using (var a = emiter.DeclareLocal<byte>())
            {
                emiter.LoadLocal(value);
                emiter.Call(typeof(ModelWithCustomCompilerAttribute).GetProperty(nameof(ModelWithCustomCompilerAttribute.A)).GetMethod);
                emiter.StoreLocal(a);

                emiter.CallSerializerForType(a.LocalType, a);
            }
        }
    }
}