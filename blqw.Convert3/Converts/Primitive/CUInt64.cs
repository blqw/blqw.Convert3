using System;
using System.Globalization;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="ulong" /> 转换器
    /// </summary>
    public class CUInt64 : SystemTypeConvertor<ulong>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override ulong ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            ulong result;
            if (ulong.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = ulong.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(ulong);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override ulong ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? 1 : (ulong) 0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(ulong);
                    case TypeCode.Byte:
                        return conv.ToByte(null);
                    case TypeCode.Char:
                        return conv.ToChar(null);
                    case TypeCode.Int16:
                    {
                        var a = conv.ToInt16(null);
                        if (a < 0)
                        {
                            success = false;
                            return default(ulong);
                        }
                        return (ulong) a;
                    }
                    case TypeCode.Int32:
                    {
                        var a = conv.ToInt32(null);
                        if (a < 0)
                        {
                            success = false;
                            return default(ulong);
                        }
                        return (ulong) a;
                    }
                    case TypeCode.Int64:
                    {
                        var a = conv.ToInt64(null);
                        if (a < 0)
                        {
                            success = false;
                            return default(ulong);
                        }
                        return (ulong) a;
                    }
                    case TypeCode.SByte:
                    {
                        var a = conv.ToSByte(null);
                        if (a < 0)
                        {
                            success = false;
                            return default(ulong);
                        }
                        return (ulong) a;
                    }
                    case TypeCode.Double:
                    {
                        var a = conv.ToDouble(null);
                        if (a < 0)
                        {
                            success = false;
                            return default(ulong);
                        }
                        return (ulong) a;
                    }
                    case TypeCode.Single:
                    {
                        var a = conv.ToSingle(null);
                        if (a < 0)
                        {
                            success = false;
                            return default(ulong);
                        }
                        return (ulong) a;
                    }
                    case TypeCode.UInt16:
                        return conv.ToUInt16(null);
                    case TypeCode.UInt32:
                        return conv.ToUInt32(null);
                    case TypeCode.UInt64:
                        return conv.ToUInt64(null);
                    case TypeCode.Decimal:
                    {
                        var a = conv.ToDecimal(null);
                        if (a < 0)
                        {
                            success = false;
                            return default(ulong);
                        }
                        return (ulong) a;
                    }
                    case TypeCode.Object:
                        break;
                    case TypeCode.String:
                        return ChangeType(context, conv.ToString(null), outputType, out success);
                    default:
                        break;
                }
            }
            else if (input is IntPtr)
            {
                var a = ((IntPtr) input).ToInt64();
                if (a < 0)
                {
                    success = false;
                    return default(ulong);
                }
                success = true;
                return (ulong) a;
            }
            else if (input is UIntPtr)
            {
                success = true;
                return ((UIntPtr) input).ToUInt64();
            }
            else
            {
                var bs = input as byte[];
                if (bs?.Length == 8)
                {
                    success = true;
                    return BitConverter.ToUInt64(bs, 0);
                }
            }
            success = false;
            return default(ulong);
        }
    }
}