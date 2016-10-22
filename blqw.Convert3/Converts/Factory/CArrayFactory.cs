using System;

namespace blqw.Converts
{
    /// <summary>
    /// 数组转换器工厂
    /// </summary>
    public class CArrayFactory : ConvertorFactory
    {
        /// <summary>
        /// 转换器的输出类型
        /// </summary>
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