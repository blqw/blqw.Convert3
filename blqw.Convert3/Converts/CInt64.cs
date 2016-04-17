using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CInt64 : SystemTypeConvertor<Int64>
    {
        protected override bool Try(object input, out long result)
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
                        result = conv.ToBoolean(null) ? (long)1 : (long)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = (long)conv.ToByte(null); return true;
                    case TypeCode.Char: result = (long)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = (long)conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = (long)conv.ToInt32(null); return true;
                    case TypeCode.Int64: result = (long)conv.ToInt64(null); return true;
                    case TypeCode.SByte: result = (long)conv.ToSByte(null); return true;
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -9223372036854775808L || a > 9223372036854775807L)
                            {
                                result = 0;
                                return false;
                            }
                            result = (long)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -9223372036854775808L || a > 9223372036854775807L)
                            {
                                result = 0;
                                return false;
                            }
                            result = (long)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = (long)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (long)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 9223372036854775807L)
                            {
                                result = 0;
                                return false;
                            }
                            result = (long)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -9223372036854775808L || a > 9223372036854775807L)
                            {
                                result = 0;
                                return false;
                            }
                            result = (long)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input is IntPtr)
            {
                result = ((IntPtr)input).ToInt64();
                return true;
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > 9223372036854775807L)
                {
                    result = 0;
                    return false;
                }
                result = (long)a;
                return true;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null && bs.Length == 8)
                {
                    result = BitConverter.ToInt64(bs, 0);
                    return true;
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out long result)
        {
            if (long.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return long.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
