using System;

namespace blqw.Converts
{
    /// <summary>
    /// 可空值类型转换器工厂
    /// </summary>
    public class CNullableFactory : GenericConvertor
    {
        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        public override Type OutputType => typeof(Nullable<>);

        /// <summary>
        /// 根据返回类型的泛型参数类型返回新的转换器
        /// </summary>
        /// <param name="outputType"> </param>
        /// <param name="genericTypes"> </param>
        /// <returns> </returns>
        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CNullable<>).MakeGenericType(genericTypes);
            return (IConvertor) Activator.CreateInstance(type);
        }
    }
}