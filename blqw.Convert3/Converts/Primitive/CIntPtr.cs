using System;

namespace blqw.Converts
{
    internal sealed class CIntPtr : SystemTypeConvertor<IntPtr>
    {
        protected override IntPtr ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            var num = context.Get<long>().ChangeType(context, input, typeof(long), out success);
            return success ? new IntPtr(num) : default(IntPtr);
        }

        protected override IntPtr ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var num = context.Get<long>().ChangeType(context, input, typeof(long), out success);
            return success ? new IntPtr(num) : default(IntPtr);
        }
    }
}