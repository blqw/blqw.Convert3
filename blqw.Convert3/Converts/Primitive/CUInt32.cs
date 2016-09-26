using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CUInt32 : SystemTypeConvertor<uint>
    {
        protected override uint ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (uint)1 : (uint)0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(uint);
                    case TypeCode.Byte: return (uint)conv.ToByte(null);
                    case TypeCode.Char: return (uint)conv.ToChar(null);
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(uint);
                            }
                            return (uint)a;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(uint);
                            }
                            return (uint)a;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > uint.MaxValue)
                            {
                                success = false;
                                return default(uint);
                            }
                            return (uint)a;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(uint);
                            }
                            return (uint)a;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > uint.MaxValue)
                            {
                                success = false;
                                return default(uint);
                            }
                            return (uint)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > uint.MaxValue)
                            {
                                success = false;
                                return default(uint);
                            }
                            return (uint)a;
                        }
                    case TypeCode.UInt16: return (uint)conv.ToUInt16(null);
                    case TypeCode.UInt32: return conv.ToUInt32(null);
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > uint.MaxValue)
                            {
                                success = false;
                                return default(uint);
                            }
                            return (uint)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > uint.MaxValue)
                            {
                                success = false;
                                return default(uint);
                            }
                            return (uint)a;
                        }
                    default:
                        break;
                }
            }
            else if (input is IntPtr)
            {
                var a = ((IntPtr)input).ToInt64();
                if (a < 0 || a > uint.MaxValue)
                {
                    success = false;
                    return default(uint);
                }
                success = true;
                return (uint)a;

            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > uint.MaxValue)
                {
                    success = false;
                    return default(uint);
                }
                success = true;
                return (uint)a;

            }
            else
            {
                var bs = input as byte[];
                if (bs != null && bs.Length == 4)
                {
                    success = true;
                    return BitConverter.ToUInt32(bs, 0);
                }
            }
            success = false;
            return default(uint);
        }

        protected override uint ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            uint result;
            if (uint.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = uint.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(uint);
        }

    }
}
