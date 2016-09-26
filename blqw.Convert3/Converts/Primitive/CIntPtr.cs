using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CIntPtr : SystemTypeConvertor<IntPtr>
    {
        protected override IntPtr ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            var num = ConvertorServices.Container.GetConvertor<long>().ChangeType(context, input, typeof(long), out success);
            return (success) ? new IntPtr(num) : default(IntPtr);
        }

        protected override IntPtr ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var num = ConvertorServices.Container.GetConvertor<long>().ChangeType(context, input, typeof(long), out success);
            return (success) ? new IntPtr(num) : default(IntPtr);
        }
    }
}
