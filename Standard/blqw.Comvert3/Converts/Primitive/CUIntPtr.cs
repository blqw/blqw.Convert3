using System;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="UIntPtr"/> 转换器
    /// </summary>
    public class CUIntPtr : SystemTypeConvertor<UIntPtr>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override UIntPtr ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            var num = context.Get<ulong>().ChangeType(context, input, typeof(ulong), out success);
            return success ? new UIntPtr(num) : default(UIntPtr);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override UIntPtr ChangeTypeImpl(IConvertContext context, object input, Type outputType,
            out bool success)
        {
            var num = context.Get<ulong>().ChangeType(context, input, typeof(ulong), out success);
            return success ? new UIntPtr(num) : default(UIntPtr);
        }
    }
}