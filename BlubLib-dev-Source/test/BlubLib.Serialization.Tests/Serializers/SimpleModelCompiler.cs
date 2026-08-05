using System;
using BlubLib.Serialization.Tests.Models;
using Sigil;
using Sigil.NonGeneric;

namespace BlubLib.Serialization.Tests.Serializers
{
    public class SimpleModelCompiler : ISerializerCompiler
    {
        public bool CanHandle(Type type) => type == typeof(SimpleModel);

        public void EmitSerialize(Emit emiter, Local value)
        {
            using (var a = emiter.DeclareLocal<int>())
            {
                emiter.LoadLocal(value);
                emiter.Call(typeof(SimpleModel).GetProperty(nameof(SimpleModel.A)).GetMethod);
                emiter.Convert<int>();
                emiter.StoreLocal(a);
                emiter.CallSerializerForType(a.LocalType, a);
            }
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.NewObject<SimpleModel>();
            emiter.StoreLocal(value);

            using (var a = emiter.DeclareLocal<int>())
            {
                emiter.CallDeserializerForType(a.LocalType, a);

                emiter.LoadLocal(value);
                emiter.LoadLocal(a);
                emiter.Convert<byte>();
                emiter.Call(typeof(SimpleModel).GetProperty(nameof(SimpleModel.A)).SetMethod);
            }
        }
    }
}
