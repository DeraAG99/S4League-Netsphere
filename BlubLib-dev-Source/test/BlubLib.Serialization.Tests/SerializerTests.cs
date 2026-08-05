using System;
using System.IO;
using BlubLib.Serialization.Tests.Models;
using BlubLib.Serialization.Tests.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Serialization.Tests
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void Serialize_WithSimpleType()
        {
            var expected = new byte[] { 3 };
            var foo = new SimpleModel { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithSimpleTypeAsObject()
        {
            var expected = new byte[] { 3 };
            var foo = new SimpleModel { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, (object)foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithInheritedType()
        {
            var expected = new byte[] { 3, 4 };
            var bar = new InheritedModel { A = 3, B = 4 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, bar);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithValueType()
        {
            var expected = new byte[] { 3 };
            var foo = new ModelStruct { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithValueTypeAsObject()
        {
            var expected = new byte[] { 3 };
            var foo = new ModelStruct { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, (object)foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithTypeWithoutAttributes_ShouldThrowException()
        {
            try
            {
                Serializer.Serialize(new MemoryStream(), new ModelWithoutAttributes());
            }
            catch (Exception)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void Serialize_WithBinaryWriterAndSimpleType()
        {
            var expected = new byte[] { 3 };
            var foo = new SimpleModel { A = 3 };
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            Serializer.Serialize(writer, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithBinaryWriterAndSimpleTypeAsObject()
        {
            var expected = new byte[] { 3 };
            var foo = new SimpleModel { A = 3 };
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            Serializer.Serialize(writer, (object)foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithCustomSerializerOnAttribute()
        {
            var expected = new byte[] { 3 };
            var foo = new ModelWithCustomSerializerAttribute { A = 3 };
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            Serializer.Serialize(writer, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithCustomCompilerOnAttribute()
        {
            var expected = new byte[] { 3 };
            var foo = new ModelWithCustomCompilerAttribute { A = 3 };
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            Serializer.Serialize(writer, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithComplexType()
        {
            var expected = new byte[] { 3 };
            var foo = new ComplexModel { SimpleModel = new SimpleModel { A = 3 } };
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            Serializer.Serialize(writer, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithCustomSerializerOnProperty()
        {
            var expected = new byte[] { 3, 0, 0, 0 };
            var foo = new ModelWithCustomSerializerOnProperty { SimpleModel = new SimpleModel { A = 3 } };
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            Serializer.Serialize(writer, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithCustomCompilerOnProperty()
        {
            var expected = new byte[] { 3, 0, 0, 0 };
            var foo = new ModelWithCustomCompilerOnProperty { SimpleModel = new SimpleModel { A = 3 } };
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            Serializer.Serialize(writer, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);
        }

        [TestMethod]
        public void Serialize_WithShouldSerializePattern()
        {
            var expected = new byte[] { 1, 2 };
            var expected2 = new byte[] { 0 };

            var foo = new ModelWithShouldSerialize { A = true, B = 2 };
            var foo2 = new ModelWithShouldSerialize { A = false, B = 2 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);

            var array = ms.ToArray();
            CollectionAssert.AreEqual(expected, array);

            ms.Position = 0;
            ms.SetLength(0);
            Serializer.Serialize(ms, foo2);

            array = ms.ToArray();
            CollectionAssert.AreEqual(expected2, array);
        }

        [TestMethod]
        public void Deserialize_WithSimpleType()
        {
            var foo = new SimpleModel { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = Serializer.Deserialize<SimpleModel>(ms);
            Assert.AreEqual(foo.A, foo2.A);
        }

        [TestMethod]
        public void Deserialize_WithSimpleTypeAsObject()
        {
            var foo = new SimpleModel { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = (SimpleModel)Serializer.Deserialize(ms, typeof(SimpleModel));
            Assert.AreEqual(foo.A, foo2.A);
        }

        [TestMethod]
        public void Deserialize_WithInheritedType()
        {
            var bar = new InheritedModel { A = 3, B = 4 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, bar);
            ms.Position = 0;

            var bar2 = Serializer.Deserialize<InheritedModel>(ms);
            Assert.AreEqual(bar.A, bar2.A);
            Assert.AreEqual(bar.B, bar2.B);
        }

        [TestMethod]
        public void Deserialize_WithValueType()
        {
            var foo = new ModelStruct { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = Serializer.Deserialize<ModelStruct>(ms);
            Assert.AreEqual(foo.A, foo2.A);
        }

        [TestMethod]
        public void Deserialize_WithValueTypeAsObject()
        {
            var foo = new ModelStruct { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = (ModelStruct)Serializer.Deserialize(ms, typeof(ModelStruct));
            Assert.AreEqual(foo.A, foo2.A);
        }

        [TestMethod]
        public void Deserialize_WithTypeWithoutAttributes_ShouldThrowException()
        {
            try
            {
                Serializer.Deserialize<ModelWithoutAttributes>(new MemoryStream());
            }
            catch (Exception)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void Deserialize_WithBinaryReaderAndSimpleType()
        {
            var foo = new SimpleModel { A = 3 };
            var ms = new MemoryStream();
            var reader = new BinaryReader(ms);
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = Serializer.Deserialize<SimpleModel>(reader);
            Assert.AreEqual(foo.A, foo2.A);
        }

        [TestMethod]
        public void Deserialize_WithBinaryReaderAndSimpleTypeAsObject()
        {
            var foo = new SimpleModel { A = 3 };
            var ms = new MemoryStream();
            var reader = new BinaryReader(ms);
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = (SimpleModel)Serializer.Deserialize(reader, typeof(SimpleModel));
            Assert.AreEqual(foo.A, foo2.A);
        }

        [TestMethod]
        public void Deserialize_WithCustomSerializerOnAttribute()
        {
            var foo = new ModelWithCustomSerializerAttribute { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = Serializer.Deserialize<ModelWithCustomSerializerAttribute>(ms);
            Assert.AreEqual(foo.A, foo2.A);
        }

        [TestMethod]
        public void Deserialize_WithCustomCompilerOnAttribute()
        {
            var foo = new ModelWithCustomCompilerAttribute { A = 3 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = Serializer.Deserialize<ModelWithCustomCompilerAttribute>(ms);
            Assert.AreEqual(foo.A, foo2.A);
        }

        [TestMethod]
        public void Deserialize_WithComplexType()
        {
            var foo = new ComplexModel { SimpleModel = new SimpleModel { A = 3 } };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = Serializer.Deserialize<ComplexModel>(ms);
            Assert.AreEqual(foo.SimpleModel.A, foo2.SimpleModel.A);
        }

        [TestMethod]
        public void Deserialize_WithWithCustomSerializerOnProperty()
        {
            var foo = new ModelWithCustomSerializerOnProperty { SimpleModel = new SimpleModel { A = 3 } };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = Serializer.Deserialize<ModelWithCustomSerializerOnProperty>(ms);
            Assert.AreEqual(foo.SimpleModel.A, foo2.SimpleModel.A);
        }

        [TestMethod]
        public void Deserialize_WithWithCustomCompilerOnProperty()
        {
            var foo = new ModelWithCustomCompilerOnProperty { SimpleModel = new SimpleModel { A = 3 } };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo2 = Serializer.Deserialize<ModelWithCustomCompilerOnProperty>(ms);
            Assert.AreEqual(foo.SimpleModel.A, foo2.SimpleModel.A);
        }

        [TestMethod]
        public void Deserialize_WithShouldSerializePattern()
        {
            var foo = new ModelWithShouldSerialize { A = true, B = 2 };
            var foo2 = new ModelWithShouldSerialize { A = false, B = 2 };
            var ms = new MemoryStream();
            Serializer.Serialize(ms, foo);
            ms.Position = 0;

            var foo3 = Serializer.Deserialize<ModelWithShouldSerialize>(ms);
            Assert.AreEqual(foo.B, foo3.B);

            ms.Position = 0;
            ms.SetLength(0);
            Serializer.Serialize(ms, foo2);
            ms.Position = 0;

            var foo4 = Serializer.Deserialize<ModelWithShouldSerialize>(ms);
            Assert.AreEqual(default(byte), foo4.B);
        }

        [TestMethod]
        public void AddSerializer_ShouldThrowArgumentException_WhenSerializerAlreadyAdded()
        {
            var serializer = new ModelWithCustomSerializerAttributeSerializer();
            Serializer.AddSerializer(serializer);

            try
            {
                Serializer.AddSerializer(serializer);
            }
            catch (Exception)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void AddCompiler_ShouldThrowArgumentException_WhenCompilerAlreadyAdded()
        {
            var serializer = new ModelWithCustomCompilerAttributeCompiler();
            Serializer.AddCompiler(serializer);

            try
            {
                Serializer.AddCompiler(serializer);
            }
            catch (Exception)
            {
                return;
            }

            Assert.Fail();
        }
    }
}
