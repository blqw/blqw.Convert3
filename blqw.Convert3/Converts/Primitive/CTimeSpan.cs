using System;

namespace blqw.Converts
{
    internal sealed class CTimeSpan : SystemTypeConvertor<TimeSpan>
    {
        private static readonly string[] _Formats = { "hhmmss", "hhmmssfff" };

        protected override TimeSpan ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            success = false;
            return default(TimeSpan);
        }

        protected override TimeSpan ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            TimeSpan result;
            success = TimeSpan.TryParse(input, out result)
                      || TimeSpan.TryParseExact(input, _Formats, null, out result);
            return result;
        }
    }
}