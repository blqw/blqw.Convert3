using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CEnumFactory : ConvertorFactory
    {
        public override Type OutputType => typeof(Enum);

        /// <summary>
        /// 获取子转换器
        /// </summary>
        protected override IConvertor GetConvertor(Type outputType)
        {
            if (outputType == null)
                throw new ArgumentNullException(nameof(outputType));
            if (outputType.IsEnum == false)
                throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}必须是枚举");
            var type = typeof(CEnum<>).MakeGenericType(outputType);
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }
    }
}
