using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public sealed class CArrayFactory : ConvertorFactory
    {
        public override Type OutputType => typeof(Array);

        /// <summary>
        /// 获取子转换器
        /// </summary>
        protected override IConvertor GetConvertor(Type outputType)
        {
            var type = typeof(CArray<>).MakeGenericType(outputType.GetElementType());
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }
    }
}
