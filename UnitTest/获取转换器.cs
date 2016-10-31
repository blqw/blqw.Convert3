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
            var convert = ConvertorServices.Container.GetConvertor(typeof(MyClass));
            Assert.IsNotNull(convert);

        }

        class MyClass
        {

        }
    }
}
