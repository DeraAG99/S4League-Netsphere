using System;
using System.IO;
using BlubLib.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests.Buffers
{
    [TestClass]
    public class BufferTests
    {
        [TestMethod]
        public void TestBufferManager()
        {
            const int bufferSize = 5;
            const int bufferCount = 2;

            var bufferMgr = new BufferManager(bufferSize, bufferCount, true);

            Assert.AreEqual(bufferCount, bufferMgr.AvailableBuffers);

            var buffer = bufferMgr.Rent();
            var buffer2 = bufferMgr.Rent();

            Assert.AreNotSame(buffer, buffer2);
            Assert.AreEqual(bufferCount - 2, bufferMgr.AvailableBuffers);
            Assert.AreEqual(bufferSize, buffer.Count);

            bufferMgr.Return(buffer);
            bufferMgr.Return(buffer2);

            Assert.AreEqual(bufferCount, bufferMgr.AvailableBuffers);
        }

        [TestMethod]
        public void TestBufferManagerGrowth()
        {
            const int bufferSize = 5;
            const int bufferCount = 2;

            var bufferMgr = new BufferManager(bufferSize, bufferCount, true);

            Assert.AreEqual(bufferCount, bufferMgr.AvailableBuffers);

            var b1 = bufferMgr.Rent();
            var b2 = bufferMgr.Rent();
            var b3 = bufferMgr.Rent();

            Assert.AreEqual(bufferCount - 1, bufferMgr.AvailableBuffers);

            bufferMgr.Return(b1);
            bufferMgr.Return(b2);
            bufferMgr.Return(b3);

            Assert.AreEqual(bufferCount*2, bufferMgr.AvailableBuffers);

            bufferMgr = new BufferManager(bufferSize, bufferCount, false);
            try
            {
                bufferMgr.Rent();
                bufferMgr.Rent();
                bufferMgr.Rent();
            }
            catch (OutOfMemoryException)
            {
                return;
            }
            Assert.Fail("BufferManager should throw OutOfMemoryException with canGrow=false");
        }

        [TestMethod]
        public void TestBuffer()
        {
            var bufferMgr = new BufferManager(5, 1, true);
            bufferMgr.Rent().Dispose();
            Assert.AreEqual(1, bufferMgr.AvailableBuffers);

            var buffer = bufferMgr.Rent();
            var count = 0;
            foreach (var _ in buffer)
                ++count;

            Assert.AreEqual(bufferMgr.BufferSize, count);
        }

        [TestMethod]
        public void TestBufferStream()
        {
            var bufferMgr = new BufferManager(1, 1, true);
            var stream = new BufferStream(bufferMgr);
            var w = stream.ToBinaryWriter(true);

            w.Write((byte) 1);
            Assert.AreEqual(1, stream.Length);

            w.Write((byte)2);
            Assert.AreEqual(2, stream.Length);

            stream.Dispose();
            
            Assert.AreEqual(2, bufferMgr.AvailableBuffers);

            stream = new BufferStream(bufferMgr);
            w = stream.ToBinaryWriter(true);
            var r = stream.ToBinaryReader(true);

            w.Write((byte)1);
            w.Write((byte)2);
            stream.Position = 0;

            Assert.AreEqual(1, r.ReadByte());
            Assert.AreEqual(1, stream.Position);

            Assert.AreEqual(2, r.ReadByte());
            Assert.AreEqual(2, stream.Position);

            CollectionAssert.AreEqual(new byte[]{1,2}, stream.ToArray());
        }
    }
}
