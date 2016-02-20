using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CSingle : SystemTypeConvertor<Single>
    {
        protected override bool Try(object input, out float result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (Single)1 : (Single)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte:
                        result = conv.ToByte(null);
                        return true;
                    case TypeCode.Char:
                        result = (Single)conv.ToChar(null);
                        return true;
                    case TypeCode.Int16: result = (Single)conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = (Single)conv.ToInt32(null); return true;
                    case TypeCode.Int64: result = (Single)conv.ToInt64(null); return true;
                    case TypeCode.SByte: result = (Single)conv.ToSByte(null); return true;
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < Single.MinValue || a > Single.MaxValue)
                            {
                                result = 0;
                                return false;
                            }
                            result = (Single)a;
                            return true;
                        }
                    case TypeCode.Single: result = (Single)conv.ToSingle(null); return true;
                    case TypeCode.UInt16: result = (Single)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (Single)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64: result = (Single)conv.ToUInt64(null); return true;
                    case TypeCode.Decimal: result = (Single)conv.ToDecimal(null); return true;
                    default:
                        break;
                }
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 4)
                    {
                        result = BitConverter.ToSingle(bs, 0);
                        return true;
                    }
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out float result)
        {
            if (float.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return float.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
