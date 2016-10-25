using System;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="IntPtr" /> 转换器
    /// </summary>
    public class CIntPtr : SystemTypeConvertor<IntPtr>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override IntPtr ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            var num = context.Get<long>().ChangeType(context, input, typeof(long), out success);
            return success ? new IntPtr(num) : default(IntPtr);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override IntPtr ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var num = context.Get<long>().ChangeType(context, input, typeof(long), out success);
            return success ? new IntPtr(num) : default(IntPtr);
        }
    }
}