using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CDateTime : SystemTypeConvertor<DateTime>
    {
        protected override bool Try(object input, out DateTime result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.DateTime:
                        result = conv.ToDateTime(null);
                        return true;
                    case TypeCode.Byte:
                    case TypeCode.Boolean:
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.Char:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Decimal:
                        result = default(DateTime);
                        return false;
                    default:
                        break;
                }
            }
            result = default(DateTime);
            return false;
        }

        static readonly string[] Formats = new[] { "yyyyMMddHHmmss", "yyyyMMddHHmmssfff" }; 

        protected override bool Try(string input, out DateTime result)
        {
            return DateTime.TryParse(input, out  result)
                || DateTime.TryParseExact(input, Formats, null, DateTimeStyles.None, out  result);
        }
    }
}
