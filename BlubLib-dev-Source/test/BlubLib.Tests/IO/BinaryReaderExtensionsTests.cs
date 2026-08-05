using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests.IO
{
    [TestClass]
    public class BinaryReaderExtensionsTests
    {
        [TestMethod]
        public void ReadToEndTest()
        {
            const int arraySize = 100;
            var data = new byte[arraySize];
            var random = new Random();
            random.NextBytes(data);

            var ms = new MemoryStream(data);
            var r = new BinaryReader(ms);
            var array = r.ReadToEnd();

            Assert.AreEqual(arraySize, array.Length);
            Assert.IsTrue(data.SequenceEqual(array));
        }

        [TestMethod]
        public void ReadToEndAsyncTest()
        {
            const int arraySize = 100;
            var data = new byte[arraySize];
            var random = new Random();
            random.NextBytes(data);

            var ms = new MemoryStream(data);
            var r = new BinaryReader(ms);
            var array = r.ReadToEndAsync().Result;

            Assert.AreEqual(arraySize, array.Length);
            Assert.IsTrue(data.SequenceEqual(array));
        }

        [TestMethod]
        public void ReadCStringTest()
        {
            const string value = "Kappa123";
            const string expected = "Kappa123";

            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.WriteCString(value);

            ms.Position = 0;
            var r = new BinaryReader(ms);
            var str = r.ReadCString();

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void ReadCStringTest_BigString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 1024; i++)
                sb.Append('a');

            var value = sb.ToString();
            var expected = value;

            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.WriteCString(value);

            ms.Position = 0;
            var r = new BinaryReader(ms);
            var str = r.ReadCString();

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void ReadCStringTest_WithLength()
        {
            const string value = "Kappa123";
            const string expected = "Kappa";

            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.WriteCString(value);

            ms.Position = 0;
            var r = new BinaryReader(ms);
            var str = r.ReadCString(5);

            Assert.AreEqual(expected, str);
            Assert.AreEqual(5, ms.Position);
        }

        [TestMethod]
        public void ReadCStringTest_WithLength_BigString()
        {
            const int length = 1024;
            const int overhead = 10;

            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
                sb.Append('a');

            var value = sb.ToString();

            for (var i = 0; i < overhead; i++)
                sb.Append('\0');
            var expected = value;

            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.WriteCString(value, length + overhead);

            ms.Position = 0;
            var r = new BinaryReader(ms);
            var str = r.ReadCString(length+overhead);

            Assert.AreEqual(expected, str);
            Assert.AreEqual(ms.Length, ms.Position);
        }

        [TestMethod]
        public void ReadCStringTest_WithLengthGreaterThanActualLength()
        {
            const string value = "Kappa123";
            const string expected = "Kappa123";

            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.WriteCString(value);
            w.Write(new byte[6]);

            ms.Position = 0;
            var r = new BinaryReader(ms);
            var str = r.ReadCString(15);

            Assert.AreEqual(expected, str);
            Assert.AreEqual(ms.Length, ms.Position);
        }
    }
}
