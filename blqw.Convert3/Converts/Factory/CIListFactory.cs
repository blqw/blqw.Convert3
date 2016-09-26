using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public sealed class CIListFactory : GenericConvertor
    {
        public override Type OutputType => typeof(ICollection<>);

        /// <summary>
        /// 根据返回类型的泛型参数类型返回新的转换器
        /// </summary>
        /// <param name="outputType"></param>
        /// <param name="genericTypes"></param>
        /// <returns></returns>
        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CIList<>).MakeGenericType(genericTypes);
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }
    }
}
