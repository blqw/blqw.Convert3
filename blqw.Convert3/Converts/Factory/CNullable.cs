using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CNullable : GenericConvertor
    {
        public override Type OutputType => typeof(Nullable<>);

        /// <summary>
        /// 根据返回类型的泛型参数类型返回新的转换器
        /// </summary>
        /// <param name="outputType"></param>
        /// <param name="genericTypes"></param>
        /// <returns></returns>
        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CNullable<>).MakeGenericType(genericTypes);
            return (IConvertor)Activator.CreateInstance(type);
        }
    }

    public class CNullable<T> : BaseTypeConvertor<T?>
        where T : struct
    {
        protected override T? ChangeType(ConvertContext context,string input, Type outputType, out bool success)
        {
            if (input.Length == 0)
            {
                success = true;
                return null;
            }
            var conv = context.Get<T>();
            return conv.ChangeType(context, input, conv.OutputType, out success);
        }

        protected override T? ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            if (input == null || input is DBNull)
            {
                success = true;
                return null;
            }

            var conv = context.Get<T>();
            return conv.ChangeType(context, input, conv.OutputType, out success);
        }
        
    }
}
