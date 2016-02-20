using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CUInt16 : SystemTypeConvertor<UInt16>
    {
        protected override bool Try(object input, out ushort result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (ushort)1 : (ushort)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = (ushort)conv.ToByte(null); return true;
                    case TypeCode.Char: result = (ushort)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = (ushort)conv.ToInt16(null); return true;
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = conv.ToUInt16(null); return true;
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else
            {
                var bs = input as byte[];
                if (bs != null && bs.Length == 2)
                {
                    result = BitConverter.ToUInt16(bs, 0);
                    return true;
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out ushort result)
        {
            if (ushort.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return ushort.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
