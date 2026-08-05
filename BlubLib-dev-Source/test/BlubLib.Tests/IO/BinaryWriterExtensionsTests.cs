using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests.IO
{
    [TestClass]
    public class BinaryWriterExtensionsTests
    {
        [TestMethod]
        public void WriteCStringTest()
        {
            const string value = "Kappa123";
            const string expected = "Kappa123\0";

            var ms = new MemoryStream();
            var r = new BinaryWriter(ms);
            r.WriteCString(value);

            var data = ms.ToArray();
            var str = Encoding.UTF8.GetString(data);

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void WriteCStringTest_BigString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 1024; i++)
                sb.Append('a');

            var value = sb.ToString();
            var expected = value + "\0";

            var ms = new MemoryStream();
            var r = new BinaryWriter(ms);
            r.WriteCString(value);

            var data = ms.ToArray();
            var str = Encoding.UTF8.GetString(data);

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void WriteCStringTest_WithMaxLength()
        {
            const string value = "Kappa123";
            const string expected = "Kappa123\0\0\0";

            var ms = new MemoryStream();
            var r = new BinaryWriter(ms);
            r.WriteCString(value, 11);

            var data = ms.ToArray();
            var str = Encoding.UTF8.GetString(data);

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void WriteCStringTest_StringGreaterThanMaxLength_ShouldThrowArgumentOutOfRangeException()
        {
            const string value = "Kappa123";

            var ms = new MemoryStream();
            var r = new BinaryWriter(ms);
            try
            {
                r.WriteCString(value, 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void WriteCStringTest_WithMaxLength_BigString()
        {
            const int length = 1024;
            const int overhead = 10;

            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
                sb.Append('a');

            var value = sb.ToString();

            for (var i = 0; i < overhead; i++)
                sb.Append('\0');
            var expected = sb.ToString();

            var ms = new MemoryStream();
            var r = new BinaryWriter(ms);
            r.WriteCString(value, length + overhead);

            var data = ms.ToArray();
            var str = Encoding.UTF8.GetString(data);

            Assert.AreEqual(expected, str);
        }
    }
}
