using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    /// <summary>
    /// 自定定义类型
    /// </summary>
    public sealed class CIDictionaryFactory : GenericConvertor
    {
        public override Type OutputType => typeof(IDictionary<,>);

        /// <summary>
        /// 根据返回类型的泛型参数类型返回新的转换器
        /// </summary>
        /// <param name="outputType"></param>
        /// <param name="genericTypes"></param>
        /// <returns></returns>
        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CIDictionary<,>).MakeGenericType(genericTypes);
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }
        
    }
}
