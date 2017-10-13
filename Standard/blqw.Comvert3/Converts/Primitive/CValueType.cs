using System;

namespace blqw.Converts
{
    /// <summary>
    /// 值类型转换器
    /// </summary>
    public class CValueType : BaseTypeConvertor<ValueType>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override ValueType ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            context.AddException("无法为值类型(struct)提供转换");
            success = false;
            return null;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected override ValueType ChangeTypeImpl(IConvertContext context, object input, Type outputType,
            out bool success)
        {
            context.AddException("无法为值类型(struct)提供转换");
            success = false;
            return null;
        }
    }
}