using System;

namespace blqw.Converts
{
    /// <summary>
    /// 可空值类型转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CNullable<T> : BaseTypeConvertor<T?>
        where T : struct
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override T? ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                success = true;
                return null;
            }
            var conv = context.Get<T>();
            return conv.ChangeType(context, input, conv.OutputType, out success);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected override T? ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            if (input.IsNull())
            {
                success = true;
                return null;
            }

            var conv = context.Get<T>();
            return conv.ChangeType(context, input, conv.OutputType, out success);
        }
    }
}