using System;
using System.Threading;
using BlubLib.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests.Caching
{
    [TestClass]
    public class MemoryCacheTests
    {
        [TestMethod]
        public void TestItemOneSecond()
        {
            var cache = new MemoryCache();
            var obj = new object();
            cache.Set("test", obj, TimeSpan.FromSeconds(1));

            Assert.AreEqual(obj, cache.Get("test"));
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.IsNull(cache.Get("test"));
        }

        [TestMethod]
        public void TestNoExpiration()
        {
            var cache = new MemoryCache();
            var obj = new object();
            cache.Set("test", obj);

            Thread.Sleep(TimeSpan.FromSeconds(2));
            Assert.AreEqual(obj, cache.Get("test"));
        }

        [TestMethod]
        public void TestRemoveAndClear()
        {
            var cache = new MemoryCache();
            var obj = new object();
            cache.Set("test", obj);
            var result = cache.Remove("test");

            Assert.IsTrue(result);
            Assert.IsNull(cache.Get("test"));

            cache.Set("test", obj);
            cache.Clear();

            Assert.IsNull(cache.Get("test"));
        }
    }
}
