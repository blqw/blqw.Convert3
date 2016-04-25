using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CDecimal : SystemTypeConvertor<decimal>
    {
        protected override decimal ChangeType(string input, Type outputType, out bool success)
        {
            decimal result;
            if (decimal.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input)
                && decimal.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result))
            {
                success = true;
                return result;
            }
            success = false;
            return default(decimal);
        }

        protected override decimal ChangeType(object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            success = true;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (Decimal)1 : (Decimal)0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(decimal);
                    case TypeCode.Byte: return conv.ToByte(null); 
                    case TypeCode.Char: return (Decimal)conv.ToChar(null); 
                    case TypeCode.Int16: return (Decimal)conv.ToInt16(null); 
                    case TypeCode.Int32: return (Decimal)conv.ToInt32(null); 
                    case TypeCode.Int64: return (Decimal)conv.ToInt64(null); 
                    case TypeCode.SByte: return (Decimal)conv.ToSByte(null); 
                    case TypeCode.Double: return (Decimal)conv.ToDouble(null); 
                    case TypeCode.Single: return (Decimal)conv.ToSingle(null); 
                    case TypeCode.UInt16: return (Decimal)conv.ToUInt16(null); 
                    case TypeCode.UInt32: return (Decimal)conv.ToUInt32(null); 
                    case TypeCode.UInt64: return (Decimal)conv.ToUInt64(null); 
                    case TypeCode.Decimal: return (Decimal)conv.ToDecimal(null); 
                    default:
                        break;
                }
            }
            success = false;
            return default(decimal);
        }
        
    }
}
