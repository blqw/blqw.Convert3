using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CInt64 : SystemTypeConvertor<long>
    {
        protected override long ChangeType(string input, Type outputType, out bool success)
        {
            long result;
            if (long.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = long.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(long);
        }

        protected override long ChangeType(object input, Type outputType, out bool success)
        {
            success = true;
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (long)1 : (long)0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(long);
                    case TypeCode.Byte: return (long)conv.ToByte(null); 
                    case TypeCode.Char: return (long)conv.ToChar(null); 
                    case TypeCode.Int16: return (long)conv.ToInt16(null); 
                    case TypeCode.Int32: return (long)conv.ToInt32(null); 
                    case TypeCode.Int64: return (long)conv.ToInt64(null); 
                    case TypeCode.SByte: return (long)conv.ToSByte(null); 
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < long.MinValue || a > long.MaxValue)
                            {
                                success = false;
                                return default(long);
                            }
                            return (long)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < long.MinValue || a > long.MaxValue)
                            {
                                success = false;
                                return default(long);
                            }
                            return (long)a;
                        }
                    case TypeCode.UInt16: return (long)conv.ToUInt16(null); 
                    case TypeCode.UInt32: return (long)conv.ToUInt32(null); 
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > long.MaxValue)
                            {
                                success = false;
                                return default(long);
                            }
                            return (long)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < long.MinValue || a > long.MaxValue)
                            {
                                success = false;
                                return default(long);
                            }
                            return (long)a;
                        }
                    default:
                        break;
                }
            }
            else if (input is IntPtr)
            {
                return ((IntPtr)input).ToInt64();
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > long.MaxValue)
                {
                    success = false;
                    return default(long);
                }
                return (long)a;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null && bs.Length == 8)
                {
                    return BitConverter.ToInt64(bs, 0);
                }
            }
            success = false;
            return default(long);
        }

    }
}
