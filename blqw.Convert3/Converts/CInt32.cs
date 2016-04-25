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
        public override int ChangeType(string input, Type outputType, out bool success)
        {
            if (input == null || input.Length == 0)
            {
                success = false;
                return 0;
            }
            int result;
            if (int.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = int.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return 0;
        }

        public override int ChangeType(object input, Type outputType, out bool success)
        {
            if (input == null)
            {
                success = false;
                return 0;
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return 0;
                    case TypeCode.Boolean: return conv.ToBoolean(null) ? (int)1 : (int)0;
                    case TypeCode.Byte: return conv.ToByte(null);
                    case TypeCode.Char: return conv.ToChar(null);
                    case TypeCode.Int16: return conv.ToInt16(null);
                    case TypeCode.Int32: return conv.ToInt32(null);
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                success = false;
                                return 0;
                            }
                            return (int)a;
                        }
                    case TypeCode.SByte: return conv.ToSByte(null);
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                success = false;
                                return 0;
                            }
                            return (int)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                success = false;
                                return 0;
                            }
                            return (int)a;
                        }
                    case TypeCode.UInt16: return conv.ToUInt16(null); 
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 2147483647)
                            {
                                success = false;
                                return 0;
                            }
                            return (int)a;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 2147483647)
                            {
                                success = false;
                                return 0;
                            }
                            return (int)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                success = false;
                                return 0;
                            }
                            return (int)a;
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
                    success = false;
                    return 0;
                }
                success = true;
                return (int)a;
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > 2147483647)
                {
                    success = false;
                    return 0;
                }
                success = true;
                return (int)a;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 4)
                    {
                        success = true;
                        return BitConverter.ToInt32(bs, 0);
                    }
                }
            }
            success = false;
            return 0;
        }
    }
}
