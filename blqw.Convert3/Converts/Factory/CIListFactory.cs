using System;
using System.Collections.Generic;

namespace blqw.Converts
{
    /// <summary>
    /// 泛型 <see cref="ICollection{T}"/> 转换器工厂
    /// </summary>
    public class CIListFactory : GenericConvertor
    {
        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        public override Type OutputType => typeof(ICollection<>);

        /// <summary>
        /// 根据返回类型的泛型参数类型返回新的转换器
        /// </summary>
        /// <param name="outputType"> </param>
        /// <param name="genericTypes"> </param>
        /// <returns> </returns>
        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CIList<>).MakeGenericType(genericTypes);
            return (IConvertor) Activator.CreateInstance(type);
        }
    }
}