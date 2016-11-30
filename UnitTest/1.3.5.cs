using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;

namespace blqw
{
    [TestClass]
    public class UnitTest7
    {
        [TestMethod]
        public void 优化单值转数组的体验()
        {
            var x = 123456;
            var a = x.To<string[]>();
            Assert.AreEqual(1, a.Length);
            Assert.AreEqual(x.ToString(), a[0]);
            var b = x.To<int[]>();
            Assert.AreEqual(1, b.Length);
            Assert.AreEqual(x, b[0]);
            var c = x.To<long[]>();
            Assert.AreEqual(1, c.Length);
            Assert.AreEqual((long)x, c[0]);
            var d = x.To<object[]>();
            Assert.AreEqual(1, d.Length);
            Assert.AreEqual(x, d[0]);


            var e = x.To<IList<string>>();
            Assert.AreEqual(1, e.Count);
            Assert.AreEqual(x.ToString(), e[0]);
            var f = x.To<IList<int>>();
            Assert.AreEqual(1, f.Count);
            Assert.AreEqual(x, f[0]);
            var g = x.To<IList<long>>();
            Assert.AreEqual(1, g.Count);
            Assert.AreEqual((long)x, g[0]);
            var h = x.To<IList<object>>();
            Assert.AreEqual(1, h.Count);
            Assert.AreEqual(x, h[0]);

            var i = x.To<IList>();
            Assert.AreEqual(1, i.Count);
            Assert.AreEqual(x, i[0]);

            var y = "2016-1-1";
            var j = y.To<DateTime[]>();
            Assert.AreEqual(1, j.Length);
            Assert.AreEqual(DateTime.Parse("2016-1-1"), j[0]);

        }

        [TestMethod]
        public void 优化错误信息()
        {
            var x = 123456;
            try
            {
                var a = x.To<IEnumerable>();
            }
            catch (Exception ex)
            {
                if (ex.Message?.StartsWith("无法创建该接口的实例") != true)
                {
                    Assert.Fail(ex.Message);
                }
            }


            try
            {
                var a = x.To<Attribute>();
            }
            catch (Exception ex)
            {
                if (ex.Message?.StartsWith("无法创建抽象类实例") != true)
                {
                    Assert.Fail(ex.Message);
                }
            }

            try
            {
                var a = x.ChangeType(typeof(Convert));
            }
            catch (Exception ex)
            {
                if (ex.Message?.StartsWith("无法为静态类型") != true &&
                    ex.Message?.EndsWith("提供转换器") != true)
                {
                    Assert.Fail(ex.Message);
                }
            }


            try
            {
                var a = x.ChangeType(typeof(List<>));
            }
            catch (Exception ex)
            {
                if (ex.Message?.StartsWith("无法为泛型定义类型") != true &&
                    ex.Message?.EndsWith("提供转换器") != true)
                {
                    Assert.Fail(ex.Message);
                }
            }
        }
    }
}
