using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CInt32 : SystemTypeConvertor<int>
    {
        protected override bool Try(object input, out int result)
        {
            if (input == null)
            {
                result = 0;
                return false;
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (int)1 : (int)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null); return true;
                    case TypeCode.Char: result = conv.ToChar(null); return true;
                    case TypeCode.Int16: result = conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = conv.ToInt32(null); return true;
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.SByte: result = (int)conv.ToSByte(null); return true;
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = (int)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input is IntPtr)
            {
                var a = ((IntPtr)input).ToInt64();
                if (a < -2147483648 || a > 2147483647)
                {
                    result = 0;
                    return false;
                }
                result = (int)a;
                return true;
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > 2147483647)
                {
                    result = 0;
                    return false;
                }
                result = (int)a;
                return true;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 4)
                    {
                        result = BitConverter.ToInt32(bs, 0);
                        return true;
                    }
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out int result)
        {
            if (input == null || input.Length == 0)
            {
                result = 0;
                return false;
            }
            if (int.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return int.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }

    }
}
