using System;
using System.Collections.Generic;

namespace blqw.Converts
{
    /// <summary>
    /// 泛型 <see cref="IDictionary{TKey,TValue}"/> 转换器工厂
    /// </summary>
    public class CIDictionaryFactory : GenericConvertor
    {
        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        public override Type OutputType => typeof(IDictionary<,>);

        /// <summary>
        /// 根据返回类型的泛型参数类型返回新的转换器
        /// </summary>
        /// <param name="outputType"> </param>
        /// <param name="genericTypes"> </param>
        /// <returns> </returns>
        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CIDictionary<,>).MakeGenericType(genericTypes);
            return (IConvertor) Activator.CreateInstance(type);
        }
    }
}