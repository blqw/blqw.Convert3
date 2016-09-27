using System;

namespace blqw.Converts
{
    internal sealed class CNullable<T> : BaseTypeConvertor<T?>
        where T : struct
    {
        protected override T? ChangeType(ConvertContext context, string input, Type outputType, out bool success)
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
            if ((input == null) || input is DBNull)
            {
                success = true;
                return null;
            }

            var conv = context.Get<T>();
            return conv.ChangeType(context, input, conv.OutputType, out success);
        }
    }
}