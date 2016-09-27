using System;

namespace blqw.Converts
{
    internal sealed class CUri : SystemTypeConvertor<Uri>
    {
        protected override Uri ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            if (input == null)
            {
                success = true;
                return null;
            }
            Uri result;
            input = input.TrimStart();
            if ((input.Length > 10) && (input[6] != '/'))
            {
                if (Uri.TryCreate("http://" + input, UriKind.Absolute, out result))
                {
                    success = true;
                    return result;
                }
            }

            if (Uri.TryCreate(input, UriKind.Absolute, out result))
            {
                success = true;
                return result;
            }

            Error.CastFail(input + "不是一个有效的url");
            success = false;
            return null;
        }

        protected override Uri ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = false;
            return null;
        }
    }
}