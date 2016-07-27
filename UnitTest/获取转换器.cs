using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace blqw
{
    [TestClass]
    public class UnitTest6
    {
        [TestMethod]
        public void 测试获取转换器()
        {
            var convert2 = ConvertorContainer.Default.Get<MyClass>();
            Assert.IsNotNull(convert2);
            var convert = ConvertorContainer.Default.Get(typeof(MyClass));
            Assert.IsNotNull(convert);

        }

        class MyClass
        {

        }
    }
}
