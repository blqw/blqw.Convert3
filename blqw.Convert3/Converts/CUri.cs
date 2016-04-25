using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CUri : SystemTypeConvertor<Uri>
    {
        protected override Uri ChangeType(string input, Type outputType, out bool success)
        {
            Uri result;
            input = input.TrimStart();
            if (input.Length > 10 && input[6] != '/')
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

        protected override Uri ChangeType(object input, Type outputType, out bool success)
        {
            success = false;
            return null;
        }

    }
}
