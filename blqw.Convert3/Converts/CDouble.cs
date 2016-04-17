using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CDouble : SystemTypeConvertor<Double>
    {
        protected override bool Try(object input, out double result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (Double)1 : (Double)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null); return true;
                    case TypeCode.Char: result = (Double)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = (Double)conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = (Double)conv.ToInt32(null); return true;
                    case TypeCode.Int64: result = (Double)conv.ToInt64(null); return true;
                    case TypeCode.SByte: result = (Double)conv.ToSByte(null); return true;
                    case TypeCode.Double: result = (Double)conv.ToDouble(null); return true;
                    case TypeCode.Single: result = (Double)conv.ToSingle(null); return true;
                    case TypeCode.UInt16: result = (Double)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (Double)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64: result = (Double)conv.ToUInt64(null); return true;
                    case TypeCode.Decimal: result = (Double)conv.ToDecimal(null); return true;
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
                        result = BitConverter.ToDouble(bs, 0);
                        return true;
                    }
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out double result)
        {
            if (double.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return double.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
