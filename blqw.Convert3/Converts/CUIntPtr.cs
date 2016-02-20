using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CUIntPtr : SystemTypeConvertor<UIntPtr>
    {
        protected override bool Try(object input, out UIntPtr result)
        {
            var conv = Convert3.GetConvertor<UInt64>();
            ulong r;
            if (conv.Try(input,null, out r))
            {
                result = new UIntPtr(r);
                return true;
            }
            result = default(UIntPtr);
            return false;
        }

        protected override bool Try(string input, out UIntPtr result)
        {
            var conv = Convert3.GetConvertor<UInt64>();
            ulong r;
            if (conv.Try(input, null, out r))
            {
                result = new UIntPtr(r);
                return true;
            }
            result = default(UIntPtr);
            return false;
        }
    }
}
