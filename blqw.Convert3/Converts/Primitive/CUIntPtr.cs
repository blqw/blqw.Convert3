using System;

namespace blqw.Converts
{
    internal sealed class CUIntPtr : SystemTypeConvertor<UIntPtr>
    {
        protected override UIntPtr ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            var num = context.Get<ulong>().ChangeType(context, input, typeof(ulong), out success);
            return success ? new UIntPtr(num) : default(UIntPtr);
        }

        protected override UIntPtr ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            var num = context.Get<ulong>().ChangeType(context, input, typeof(ulong), out success);
            return success ? new UIntPtr(num) : default(UIntPtr);
        }
    }
}