using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests
{
    [TestClass]
    public class ObjectPoolTests
    {
        [TestMethod]
        public void ObjectPoolTest_ShouldThrowArgumentOutOfRangeException1()
        {
            try
            {
                new ObjectPool<Foo>(-1, 1, () => new Foo(1));
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void ObjectPoolTest_ShouldThrowArgumentOutOfRangeException2()
        {
            try
            {
                new ObjectPool<Foo>(0, 0, () => new Foo(1));
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void ObjectPoolTest_ShouldThrowArgumentNullException()
        {
            try
            {
                new ObjectPool<Foo>(0, 1, null);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void NewTest()
        {
            var pool = new ObjectPool<Foo>(1, 1, () => new Foo(123), f => f.Reset(), f => f.Dispose());
            var obj = pool.Rent();

            Assert.AreEqual(123, obj.State);

            pool.Return(obj);
            var obj2 = pool.Rent();

            Assert.AreEqual(obj, obj2);
            Assert.AreEqual(0, pool.Count);
        }

        [TestMethod]
        public void FreeTest()
        {
            var pool = new ObjectPool<Foo>(1, 1, () => new Foo(123), f => f.Reset(), f => f.Dispose());
            var obj = pool.Rent();
            pool.Return(obj);

            Assert.AreEqual(0, obj.State);
            Assert.AreEqual(false, obj.IsDisposed);
            Assert.AreEqual(1, pool.Count);
        }

        [TestMethod]
        public void FreeTest_OverMaximumSize()
        {
            var pool = new ObjectPool<Foo>(1, 1, () => new Foo(123), f => f.Reset(), f => f.Dispose());
            pool.Return(pool.Rent());
            var obj = new Foo(123);
            pool.Return(obj);

            Assert.AreEqual(0, obj.State);
            Assert.AreEqual(true, obj.IsDisposed);
            Assert.AreEqual(1, pool.Count);
        }

        [TestMethod]
        public void FlushTest()
        {
            var pool = new ObjectPool<Foo>(1, 2, () => new Foo(123), f => f.Reset(), f => f.Dispose());
            pool.Return(new Foo(123));
            pool.Return(new Foo(123));
            pool.Flush();

            Assert.AreEqual(pool.MinimumSize, pool.Count);
        }

        [TestMethod]
        public void DisposeTest()
        {
            var pool = new ObjectPool<Foo>(1, 1, () => new Foo(123), f => f.Reset(), f => f.Dispose());
            var obj = pool.Rent();
            pool.Return(obj);
            pool.Dispose();

            Assert.AreEqual(0, obj.State);
            Assert.AreEqual(true, obj.IsDisposed);
            Assert.AreEqual(0, pool.Count);
        }

        private class Foo : IDisposable
        {
            public int State { get; private set; }
            public bool IsDisposed { get; private set; }

            public Foo(int state)
            {
                State = state;
            }

            public void Reset()
            {
                State = 0;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}