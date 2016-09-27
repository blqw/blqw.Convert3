using System;
using System.Globalization;

namespace blqw.Converts
{
    internal sealed class CInt64 : SystemTypeConvertor<long>
    {
        protected override long ChangeType(ConvertContext context, string input, Type outputType, out bool success)
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

        protected override long ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? 1 : 0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(long);
                    case TypeCode.Byte:
                        return conv.ToByte(null);
                    case TypeCode.Char:
                        return conv.ToChar(null);
                    case TypeCode.Int16:
                        return conv.ToInt16(null);
                    case TypeCode.Int32:
                        return conv.ToInt32(null);
                    case TypeCode.Int64:
                        return conv.ToInt64(null);
                    case TypeCode.SByte:
                        return conv.ToSByte(null);
                    case TypeCode.Double:
                    {
                        var a = conv.ToDouble(null);
                        if ((a < long.MinValue) || (a > long.MaxValue))
                        {
                            success = false;
                            return default(long);
                        }
                        return (long) a;
                    }
                    case TypeCode.Single:
                    {
                        var a = conv.ToSingle(null);
                        if ((a < long.MinValue) || (a > long.MaxValue))
                        {
                            success = false;
                            return default(long);
                        }
                        return (long) a;
                    }
                    case TypeCode.UInt16:
                        return conv.ToUInt16(null);
                    case TypeCode.UInt32:
                        return conv.ToUInt32(null);
                    case TypeCode.UInt64:
                    {
                        var a = conv.ToUInt64(null);
                        if (a > long.MaxValue)
                        {
                            success = false;
                            return default(long);
                        }
                        return (long) a;
                    }
                    case TypeCode.Decimal:
                    {
                        var a = conv.ToDecimal(null);
                        if ((a < long.MinValue) || (a > long.MaxValue))
                        {
                            success = false;
                            return default(long);
                        }
                        return (long) a;
                    }
                    default:
                        break;
                }
            }
            else if (input is IntPtr)
            {
                return ((IntPtr) input).ToInt64();
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr) input).ToUInt64();
                if (a > long.MaxValue)
                {
                    success = false;
                    return default(long);
                }
                return (long) a;
            }
            else
            {
                var bs = input as byte[];
                if (bs?.Length == 8)
                {
                    return BitConverter.ToInt64(bs, 0);
                }
            }
            success = false;
            return default(long);
        }
    }
}