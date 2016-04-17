using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CIntPtr : SystemTypeConvertor<IntPtr>
    {
        protected override bool Try(object input, out IntPtr result)
        {
            var conv = Convert3.GetConvertor<Int64>();
            long r;
            if (conv.Try(input, null, out r))
            {
                result = new IntPtr(r);
                return true;
            }
            result = default(IntPtr);
            return false;
        }

        protected override bool Try(string input, out IntPtr result)
        {
            var conv = Convert3.GetConvertor<Int64>();
            long r;
            if (conv.Try(input, null, out r))
            {
                result = new IntPtr(r);
                return true;
            }
            result = default(IntPtr);
            return false;
        }
    }
}
