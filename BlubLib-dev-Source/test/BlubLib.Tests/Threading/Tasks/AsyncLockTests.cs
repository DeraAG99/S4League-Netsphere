using System.Threading;
using BlubLib.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlubLib.Tests.Threading.Tasks
{
    [TestClass]
    public class AsyncLockTests
    {
        [TestMethod]
        public void LockTest()
        {
            var l = new AsyncLock();
            var shouldTake = false;
            new Thread(() =>
            {
                using (l.Lock())
                {
                    Thread.Sleep(1000);
                    shouldTake = true;
                }
            }).Start();

            Thread.Sleep(100);
            using (l.Lock())
            {
                if(!shouldTake)
                    Assert.Fail("Got lock without unlock");
            }
        }
    }
}
