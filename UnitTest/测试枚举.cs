using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace blqw
{
    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void 测试枚举()
        {
            {
                var a = 0;
                var b = a.To<MyEnum>();
                Assert.AreEqual((MyEnum)0, b);
            }
            {
                var a = 1;
                var b = a.To<MyEnum>();
                Assert.AreEqual(MyEnum.A, b);
            }
            {
                var a = 3;
                var b = a.To<MyEnum>(0);
                Assert.AreEqual((MyEnum)0, b);
            }
        }

        [TestMethod]
        public void 测试可空枚举()
        {
            var a = 0;
            var b = a.To<MyEnum?>();
            Assert.AreEqual((MyEnum?)0, b);
            object c = null;
            var d = c.To<MyEnum?>();
            Assert.AreEqual(null, d);

        }

        [TestMethod]
        public void 测试枚举属性()
        {
            {
                var a = new { E = 0 };
                var b = a.To<MyClass>();
                Assert.AreEqual((MyEnum)a.E, b.E);
            }
            {
                var a = new { E = 1 };
                var b = a.To<MyClass>();
                Assert.AreEqual(MyEnum.A, b.E);
            }
            {
                var a = new { E = 3 };
                var b = a.To<MyClass>(null);
                Assert.AreEqual(null, b);
            }
        }

        [TestMethod]
        public void 测试可空枚举属性()
        {
            var a = new { E2 = (MyEnum?)null };
            var b = a.To<MyClass>();
            Assert.AreEqual(a.E2, b.E2);


            var c = new { E2 = 0 };
            var d = c.To<MyClass>();
            Assert.AreEqual((MyEnum)0, d.E2);
        }
        class MyClass
        {
            public MyEnum E { get; set; }
            public MyEnum? E2 { get; set; }
        }

        enum MyEnum
        {
            A = 1,
            B = 2,
        }
    }

}
