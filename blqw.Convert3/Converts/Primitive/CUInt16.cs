using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CUInt16 : SystemTypeConvertor<ushort>
    {
        protected override ushort ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            ushort result;
            if (ushort.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = ushort.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(ushort);
        }

        protected override ushort ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (ushort)1 : (ushort)0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(ushort);
                    case TypeCode.Byte: return (ushort)conv.ToByte(null);
                    case TypeCode.Char: return (ushort)conv.ToChar(null);
                    case TypeCode.Int16: return (ushort)conv.ToInt16(null);
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 65535)
                            {
                                success = false;
                                return default(ushort);
                            }
                            return (ushort)a;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 65535)
                            {
                                success = false;
                                return default(ushort);
                            }
                            return (ushort)a;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(ushort);
                            }
                            return (ushort)a;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 65535)
                            {
                                success = false;
                                return default(ushort);
                            }
                            return (ushort)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 65535)
                            {
                                success = false;
                                return default(ushort);
                            }
                            return (ushort)a;
                        }
                    case TypeCode.UInt16: return conv.ToUInt16(null);
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 65535)
                            {
                                success = false;
                                return default(ushort);
                            }
                            return (ushort)a;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 65535)
                            {
                                success = false;
                                return default(ushort);
                            }
                            return (ushort)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 65535)
                            {
                                success = false;
                                return default(ushort);
                            }
                            return (ushort)a;
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
                    success = true;
                    return BitConverter.ToUInt16(bs, 0);

                }
            }
            success = false;
            return default(ushort);
        }
    }
}
