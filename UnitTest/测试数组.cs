using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace blqw
{
    [TestClass]
    public class UnitTest4
    {
        [TestMethod]
        public void 测试数组转型()
        {
            var arr = new[] { "1", "2", "3" };
            var arr2 = arr.To<int[]>();
            Assert.AreEqual(3, arr2?.Length);
            Assert.AreEqual(1, arr2[0]);
            Assert.AreEqual(2, arr2[1]);
            Assert.AreEqual(3, arr2[2]);
        }
    }
}
