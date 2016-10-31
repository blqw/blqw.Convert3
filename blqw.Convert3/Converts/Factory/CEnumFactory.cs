using System;

namespace blqw.Converts
{
    /// <summary>
    /// 枚举转换器工厂
    /// </summary>
    public class CEnumFactory : ConvertorFactory
    {
        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        public override Type OutputType => typeof(Enum);

        /// <summary>
        /// 获取子转换器
        /// </summary>
        protected override IConvertor GetConvertor(Type outputType)
        {
            if (outputType == null)
            {
                throw new ArgumentNullException(nameof(outputType));
            }
            if (outputType.IsEnum == false)
            {
                throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}必须是枚举");
            }
            var type = typeof(CEnum<>).MakeGenericType(outputType);
            return (IConvertor) Activator.CreateInstance(type);
        }
    }
}