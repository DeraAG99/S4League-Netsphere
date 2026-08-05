using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests
{
    [TestClass]
    public class DynamicCastTests
    {
        [TestMethod]
        public void FromTest_IntToShort()
        {
            var a = 123;
            var b = DynamicCast<short>.From(a);

            Assert.IsInstanceOfType(b, typeof(short));
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void FromTest_EnumToInt()
        {
            var a = Foo.Bar;
            var b = DynamicCast<int>.From(a);

            Assert.IsInstanceOfType(b, typeof(int));
            Assert.AreEqual((int)a, b);
        }

        [TestMethod]
        public void FromTest_IntToEnum()
        {
            var a = 1;
            var b = DynamicCast<Foo>.From(a);

            Assert.IsInstanceOfType(b, typeof(Foo));
            Assert.AreEqual(Foo.Bar, b);
        }

        private enum Foo
        {
            Bar = 1
        }
    }
}