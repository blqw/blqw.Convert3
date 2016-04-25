using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CDouble : SystemTypeConvertor<double>
    {
        protected override double ChangeType(string input, Type outputType, out bool success)
        {
            double result;
            if (double.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input)
                && double.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result))
            {
                success = true;
                return result;
            }
            success = false;
            return default(double);
        }

        protected override double ChangeType(object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (Double)1 : (Double)0;   
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(double);
                    case TypeCode.Byte: return conv.ToByte(null); 
                    case TypeCode.Char: return (Double)conv.ToChar(null); 
                    case TypeCode.Int16: return (Double)conv.ToInt16(null); 
                    case TypeCode.Int32: return (Double)conv.ToInt32(null); 
                    case TypeCode.Int64: return (Double)conv.ToInt64(null); 
                    case TypeCode.SByte: return (Double)conv.ToSByte(null); 
                    case TypeCode.Double: return (Double)conv.ToDouble(null); 
                    case TypeCode.Single: return (Double)conv.ToSingle(null); 
                    case TypeCode.UInt16: return (Double)conv.ToUInt16(null); 
                    case TypeCode.UInt32: return (Double)conv.ToUInt32(null); 
                    case TypeCode.UInt64: return (Double)conv.ToUInt64(null); 
                    case TypeCode.Decimal: return (Double)conv.ToDecimal(null); 
                    default:
                        break;
                }
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 8)
                    {
                        success = true;
                        return BitConverter.ToDouble(bs, 0);
                    }
                }
            }
            success = false;
            return default(double);
        }
    }
}
