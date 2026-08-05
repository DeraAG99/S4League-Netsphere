using System.IO;
using BlubLib.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests.IO
{
    [TestClass]
    public class NonClosingStreamTests
    {
        [TestMethod]
        public void TestNonClosingStream()
        {
            var stream = new TestStream();
            new NonClosingStream(stream).Dispose();

            Assert.IsFalse(stream.DisposeCalled);
        }

        private class TestStream : Stream
        {
            public bool DisposeCalled { get; private set; }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    DisposeCalled = true;
                base.Dispose(disposing);
            }

            public override bool CanRead { get; }
            public override bool CanSeek { get; }
            public override bool CanWrite { get; }
            public override long Length { get; }
            public override long Position { get; set; }

            public override void Flush()
            {
                throw new System.NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new System.NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new System.NotImplementedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new System.NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
