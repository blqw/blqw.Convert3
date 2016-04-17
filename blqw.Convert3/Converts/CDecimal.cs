using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CDecimal : SystemTypeConvertor<Decimal>
    {
        protected override bool Try(object input, out decimal result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (Decimal)1 : (Decimal)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null); return true;
                    case TypeCode.Char: result = (Decimal)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = (Decimal)conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = (Decimal)conv.ToInt32(null); return true;
                    case TypeCode.Int64: result = (Decimal)conv.ToInt64(null); return true;
                    case TypeCode.SByte: result = (Decimal)conv.ToSByte(null); return true;
                    case TypeCode.Double: result = (Decimal)conv.ToDouble(null); return true;
                    case TypeCode.Single: result = (Decimal)conv.ToSingle(null); return true;
                    case TypeCode.UInt16: result = (Decimal)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (Decimal)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64: result = (Decimal)conv.ToUInt64(null); return true;
                    case TypeCode.Decimal: result = (Decimal)conv.ToDecimal(null); return true;
                    default:
                        break;
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out decimal result)
        {
            if (decimal.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return decimal.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
