using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CTimeSpan : SystemTypeConvertor<TimeSpan>
    {
        protected override bool Try(object input, out TimeSpan result)
        {
            result = default(TimeSpan);
            return false;
        }
        static readonly string[] Formats = new[] { "hhmmss", "hhmmssfff" }; 
        protected override bool Try(string input, out TimeSpan result)
        {
            return TimeSpan.TryParse(input, out  result)
                || TimeSpan.TryParseExact(input, Formats, null, out  result);
        }
    }
}
