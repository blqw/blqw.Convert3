using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CDateTime : SystemTypeConvertor<DateTime>
    {
        static readonly string[] Formats = new[] { "yyyyMMddHHmmss", "yyyyMMddHHmmssfff" }; 
        
        protected override DateTime ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.DateTime:
                        success = true;
                        return conv.ToDateTime(null);
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
                        success = false;
                        return default(DateTime);
                    default:
                        break;
                }
            }
            success = false;
            return default(DateTime);
        }

        protected override DateTime ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            DateTime result;
            if (DateTime.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (DateTime.TryParseExact(input, Formats, null, DateTimeStyles.None, out result))
            {
                success = true;
                return result;
            }
            success = false;
            return default(DateTime);
        }
    }
}
