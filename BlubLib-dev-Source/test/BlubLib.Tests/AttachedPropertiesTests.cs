using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests
{
    [TestClass]
    public class AttachedPropertiesTests
    {
        [TestMethod]
        public void TestAdd()
        {
            var obj = new object();
            var props = obj.GetAttachedProperties();
            props["test"] = 123;

            Assert.AreEqual(123, obj.GetProperty<int>("test"));
            Assert.AreEqual(123, obj.GetProperty("test"));
            Assert.AreEqual(123, obj.GetAttachedProperties()["test"]);
            Assert.AreEqual(123, ((dynamic)obj.GetAttachedProperties()).test);
        }

        [TestMethod]
        public void TestRemove()
        {
            var obj = new object();
            var props = obj.GetAttachedProperties();
            props["test"] = 123;
            props.Clear();

            Assert.AreEqual(0, obj.GetAttachedProperties().Count);
        }
    }
}
