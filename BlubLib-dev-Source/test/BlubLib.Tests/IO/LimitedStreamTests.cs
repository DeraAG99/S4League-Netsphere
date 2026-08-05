using System;
using System.IO;
using BlubLib.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests.IO
{
    [TestClass]
    public class LimitedStreamTests
    {
        [TestMethod]
        public void TestSeek()
        {
            var memory = new MemoryStream();
            memory.SetLength(5);
            memory.Position = 0;

            var limited = new LimitedStream(memory, 1, 1);
            limited.Seek(1, SeekOrigin.Current);

            Assert.AreEqual(1, limited.Position);
            Assert.AreEqual(0, memory.Position);

            limited.Seek(15, SeekOrigin.Current);
            Assert.AreEqual(1, limited.Position);
            Assert.AreEqual(0, memory.Position);
        }

        [TestMethod]
        public void TestRead()
        {
            var memory = new MemoryStream();
            memory.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
            memory.Position = 0;

            var limited = new LimitedStream(memory, 1, 1);
            var r = limited.ToBinaryReader(false);

            var val = r.ReadByte();
            Assert.AreEqual(2, val);
            Assert.AreEqual(1, limited.Position);
            Assert.AreEqual(0, memory.Position);

            try
            {
                r.ReadByte();
            }
            catch (EndOfStreamException)
            {
                return;
            }

            Assert.Fail("Shouldn't be able to reach past limited bounds");
        }

        //[TestMethod]
        //public void TestWrite()
        //{
        //    var memory = new MemoryStream();
        //    memory.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
        //    memory.Position = 0;

        //    var limited = new LimitedStream(memory, 1, 1);
        //    var w = limited.ToBinaryWriter(false);

        //    w.Write((byte)9);
        //    memory.Position = 1;
        //    var val = memory.ReadByte();

        //    Assert.AreEqual(9, val);
        //    Assert.AreEqual(1, limited.Position);
        //    Assert.AreEqual(0, memory.Position);

        //    try
        //    {
        //        w.Write(0);
        //    }
        //    catch (EndOfStreamException)
        //    {
        //        return;
        //    }

        //    Assert.Fail("Shouldn't be able to reach past limited bounds");
        //}
    }
}
