using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Runtime.Hosting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Activation;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace blqw.Converts
{
    internal sealed class CArrayFactory : ConvertorFactory
    {
        public override Type OutputType => typeof(Array);

        /// <summary>
        /// 获取子转换器
        /// </summary>
        protected override IConvertor GetConvertor(Type outputType)
        {
            var type = typeof(CArray<>).MakeGenericType(outputType.GetElementType());
            return (IConvertor) Activator.CreateInstance(type);
        }
    }
}