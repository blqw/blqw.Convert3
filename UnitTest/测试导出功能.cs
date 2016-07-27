using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.ComponentModel.Composition;
using blqw.IOC;

namespace blqw
{
    [TestClass]
    public class UnitTest5
    {
        [Import()]
        IFormatterConverter _Converter1;

        [Import("Convert3")]
        IFormatterConverter _Converter2;

        [TestMethod]
        public void 测试导出IFormatterConverter()
        {
            Assert.IsNull(_Converter1);
            Assert.IsNull(_Converter2);
            MEF.Import(this);
            Assert.IsNotNull(_Converter1);
            Assert.IsNotNull(_Converter2);
        }
    }
}
