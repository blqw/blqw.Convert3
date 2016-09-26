using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CTimeSpan : SystemTypeConvertor<TimeSpan>
    {
        static readonly string[] Formats = new[] { "hhmmss", "hhmmssfff" }; 
        protected override TimeSpan ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = false;
            return default(TimeSpan);
        }

        protected override TimeSpan ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            TimeSpan result;
            success = TimeSpan.TryParse(input, out result)
                || TimeSpan.TryParseExact(input, Formats, null, out result);
            return result;
        }
    }
}
