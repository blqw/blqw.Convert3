using Microsoft.VisualStudio.TestTools.UnitTesting;
using blqw.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Dynamic.Tests
{
    [TestClass()]
    public class DynamicPrimitiveTests
    {
        [TestMethod()]
        public void 测试动态类型属性()
        {
            var a = "aaa".ToDynamic();
            Assert.IsTrue(a.Length == 3);
            Assert.IsTrue(a.Length > 2);
            Assert.IsTrue(a.Length < 4);

            var b = 1.ToDynamic();
            Assert.IsTrue(b.Length == null);

            var c = true.ToDynamic();
            Assert.IsTrue(c == true);
            Assert.IsFalse(!c);

        }
    }
}