using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests
{
    [TestClass]
    public class FastActivatorTests
    {
        [TestMethod]
        public void CreateTest()
        {
            var obj = FastActivator<Foo>.Create();

            Assert.AreNotEqual(null, obj);
            Assert.AreEqual(true, obj.Bar);
        }

        [TestMethod]
        public void CreateArrayTest()
        {
            var length = 10;
            var arr = FastActivator<int>.CreateArray(length);

            Assert.AreEqual(length, arr.Length);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Foo
        {
            public bool Bar { get; }

            public Foo()
            {
                Bar = true;
            }
        }
    }
}