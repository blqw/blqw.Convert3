using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CUIntPtr : SystemTypeConvertor<UIntPtr>
    {
        protected override UIntPtr ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            var num = ConvertorServices.Container.GetConvertor<ulong>().ChangeType(context, input, typeof(ulong), out success);
            return (success) ? new UIntPtr(num) : default(UIntPtr);
        }

        protected override UIntPtr ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var num = ConvertorServices.Container.GetConvertor<ulong>().ChangeType(context, input, typeof(ulong), out success);
            return (success) ? new UIntPtr(num) : default(UIntPtr);
        }
    }
}
