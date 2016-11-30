using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace blqw
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void 测试转接口()
        {
            var arr = "1,2,3,4,5,6".To<IList<int>>();
            Assert.IsNotNull(arr);
            Assert.AreEqual(6, arr.Count);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(2, arr[1]);
            Assert.AreEqual(3, arr[2]);
            Assert.AreEqual(4, arr[3]);
            Assert.AreEqual(5, arr[4]);
            Assert.AreEqual(6, arr[5]);

            var arr2 = "1,2,3,4,5,6".To<IList>();
            Assert.IsNotNull(arr2);
            Assert.AreEqual(6, arr2.Count);
            Assert.AreEqual("1", arr2[0]);
            Assert.AreEqual("2", arr2[1]);
            Assert.AreEqual("3", arr2[2]);
            Assert.AreEqual("4", arr2[3]);
            Assert.AreEqual("5", arr2[4]);
            Assert.AreEqual("6", arr2[5]);

            var dict = new Dictionary<string, string[]>()
            {
                [" 2015-1-1"] = new[] { "1", "2", "3", "4" }
            };

            var dict2 = dict.To<IDictionary<DateTime, List<int>>>();
            Assert.IsNotNull(dict2);
            Assert.AreEqual(1, dict2.Count);
            Assert.AreEqual(new DateTime(2015, 1, 1), dict2.Keys.First());
            Assert.AreEqual(4, dict2.Values.First().Count);
            Assert.AreEqual(1, dict2.Values.First()[0]);
            Assert.AreEqual(2, dict2.Values.First()[1]);
            Assert.AreEqual(3, dict2.Values.First()[2]);
            Assert.AreEqual(4, dict2.Values.First()[3]);

            var nv = new NameValueCollection()
            {
                [" 2015-1-1"] = "1,2,3,4"
            };

            var dict3 = nv.To<IDictionary>();
            Assert.IsNotNull(dict3);
            Assert.AreEqual(1, dict3.Count);
            Assert.AreEqual(" 2015-1-1", dict3.Keys.Cast<object>().First());
            Assert.AreEqual("1,2,3,4", dict3.Values.Cast<object>().First());

        }
        
    }
}
