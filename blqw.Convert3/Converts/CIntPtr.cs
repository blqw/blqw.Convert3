using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CIntPtr : SystemTypeConvertor<IntPtr>
    {
        protected override IntPtr ChangeType(string input, Type outputType, out bool success)
        {
            var conv = ConvertorContainer.Int64Convertor;
            var num = ConvertorContainer.Int64Convertor.ChangeType(input, typeof(long), out success);
            return (success) ? new IntPtr(num) : default(IntPtr);
        }

        protected override IntPtr ChangeType(object input, Type outputType, out bool success)
        {
            var conv = ConvertorContainer.Int64Convertor;
            var num = ConvertorContainer.Int64Convertor.ChangeType(input, typeof(long), out success);
            return (success) ? new IntPtr(num) : default(IntPtr);
        }
    }
}
