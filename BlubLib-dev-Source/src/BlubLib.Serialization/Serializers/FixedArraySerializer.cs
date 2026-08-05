using System;
using System.IO;
using Sigil;
using Sigil.NonGeneric;

namespace BlubLib.Serialization.Serializers
{
    public class FixedArraySerializer : ISerializerCompiler
    {
        private readonly int _length;

        public FixedArraySerializer(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            _length = length;
        }

        public bool CanHandle(Type type) => type.IsArray;

        public void EmitDeserialize(Emit emiter, Local value)
        {
            var elementType = value.LocalType.GetElementType();

            if (_length <= 0)
            {
                emiter.Call(typeof(Array)
                    .GetMethod(nameof(Array.Empty))
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(elementType));
                emiter.StoreLocal(value);
            }
            else
            {
                // Little optimization for byte arrays
                if (elementType == typeof(byte))
                {
                    // value = reader.ReadBytes(length);
                    emiter.LoadArgument(1);
                    emiter.LoadConstant(_length);
                    emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes)));
                    emiter.StoreLocal(value);
                }
                else
                {
                    var loop = emiter.DefineLabel();
                    var loopCheck = emiter.DefineLabel();

                    // value = new [length]
                    emiter.LoadConstant(_length);
                    emiter.NewArray(elementType);
                    emiter.StoreLocal(value);

                    using (var element = emiter.DeclareLocal(elementType, "element"))
                    using (var i = emiter.DeclareLocal<int>("i"))
                    {
                        emiter.MarkLabel(loop);
                        emiter.CallDeserializerForType(elementType, element);

                        // value[i] = element
                        emiter.LoadLocal(value);
                        emiter.LoadLocal(i);
                        emiter.LoadLocal(element);
                        emiter.StoreElement(elementType);

                        // ++i
                        emiter.LoadLocal(i);
                        emiter.LoadConstant(1);
                        emiter.Add();
                        emiter.StoreLocal(i);

                        // i < length
                        emiter.MarkLabel(loopCheck);
                        emiter.LoadLocal(i);
                        emiter.LoadConstant(_length);
                        emiter.BranchIfLess(loop);
                    }
                }
            }
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            var elementType = value.LocalType.GetElementType();
            if (_length <= 0)
                return;

            var loop = emiter.DefineLabel();
            var loopCheck = emiter.DefineLabel();

            using (var element = emiter.DeclareLocal(elementType, "element"))
            using (var i = emiter.DeclareLocal<int>("i"))
            {
                emiter.Branch(loopCheck);
                emiter.MarkLabel(loop);

                // element = value[i]
                emiter.LoadLocal(value);
                emiter.LoadLocal(i);
                emiter.LoadElement(elementType);
                emiter.StoreLocal(element);

                emiter.CallSerializerForType(elementType, element);

                // ++i
                emiter.LoadLocal(i);
                emiter.LoadConstant(1);
                emiter.Add();
                emiter.StoreLocal(i);

                // i < length
                emiter.MarkLabel(loopCheck);
                emiter.LoadLocal(i);
                emiter.LoadConstant(_length);
                emiter.BranchIfLess(loop);
            }
        }
    }
}
