using System;
using System.Globalization;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="short"/> 转换器
    /// </summary>
    public class CInt16 : SystemTypeConvertor<short>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override short ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            short result;
            if (short.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = short.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(short);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override short ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (short) 1 : (short) 0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(short);
                    case TypeCode.Byte:
                        return conv.ToByte(null);
                    case TypeCode.Char:
                        return (short) conv.ToChar(null);
                    case TypeCode.Int16:
                        return conv.ToInt16(null);
                    case TypeCode.Int32:
                    {
                        var a = conv.ToInt32(null);
                        if ((a < -32768) || (a > 32767))
                        {
                            success = false;
                            return default(short);
                        }
                        return (short) a;
                    }
                    case TypeCode.Int64:
                    {
                        var a = conv.ToInt64(null);
                        if ((a < -32768) || (a > 32767))
                        {
                            success = false;
                            return default(short);
                        }
                        return (short) a;
                    }
                    case TypeCode.SByte:
                        return conv.ToSByte(null);
                    case TypeCode.Double:
                    {
                        var a = conv.ToDouble(null);
                        if ((a < -32768) || (a > 32767))
                        {
                            success = false;
                            return default(short);
                        }
                        return (short) a;
                    }
                    case TypeCode.Single:
                    {
                        var a = conv.ToSingle(null);
                        if ((a < -32768) || (a > 32767))
                        {
                            success = false;
                            return default(short);
                        }
                        return (short) a;
                    }
                    case TypeCode.UInt16:
                    {
                        var a = conv.ToUInt16(null);
                        if (a > 32767)
                        {
                            success = false;
                            return default(short);
                        }
                        return (short) a;
                    }
                    case TypeCode.UInt32:
                    {
                        var a = conv.ToUInt32(null);
                        if (a > 32767)
                        {
                            success = false;
                            return default(short);
                        }
                        return (short) a;
                    }
                    case TypeCode.UInt64:
                    {
                        var a = conv.ToUInt64(null);
                        if (a > 32767)
                        {
                            success = false;
                            return default(short);
                        }
                        return (short) a;
                    }
                    case TypeCode.Decimal:
                    {
                        var a = conv.ToDecimal(null);
                        if ((a < -32768) || (a > 32767))
                        {
                            success = false;
                            return default(short);
                        }
                        return (short) a;
                    }
                    case TypeCode.Object:
                        break;
                    case TypeCode.String:
                        return ChangeType(context, conv.ToString(null), outputType, out success);
                    default:
                        break;
                }
            }
            else
            {
                var bs = input as byte[];
                if (bs?.Length == 2)
                {
                    return BitConverter.ToInt16(bs, 0);
                }
            }
            success = false;
            return default(short);
        }
    }
}