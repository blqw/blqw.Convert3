using System;

namespace blqw.Converts
{
    internal sealed class CEnumFactory : ConvertorFactory
    {
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