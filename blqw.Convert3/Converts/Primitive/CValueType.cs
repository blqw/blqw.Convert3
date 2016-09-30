using System;

namespace blqw.Converts
{
    internal sealed class CValueType : BaseTypeConvertor<ValueType>
    {
        protected override ValueType ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            context.AddException("无法为值类型(struct)提供转换");
            success = false;
            return null;
        }

        protected override ValueType ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            context.AddException("无法为值类型(struct)提供转换");
            success = false;
            return null;
        }
    }
}