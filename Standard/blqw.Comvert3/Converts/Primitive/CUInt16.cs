using System;
using System.Globalization;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="ushort" /> 转换器
    /// </summary>
    public class CUInt16 : SystemTypeConvertor<ushort>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override ushort ChangeType(IConvertContext context, string input, Type outputType, out bool success)
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

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override ushort ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (ushort) 1 : (ushort) 0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(ushort);
                    case TypeCode.Byte:
                        return conv.ToByte(null);
                    case TypeCode.Char:
                        return conv.ToChar(null);
                    case TypeCode.Int16:
                        return (ushort) conv.ToInt16(null);
                    case TypeCode.Int32:
                    {
                        var a = conv.ToInt32(null);
                        if ((a < ushort.MinValue) || (a > ushort.MaxValue))
                        {
                            success = false;
                            return default(ushort);
                        }
                        return (ushort) a;
                    }
                    case TypeCode.Int64:
                    {
                        var a = conv.ToInt64(null);
                        if ((a < ushort.MinValue) || (a > ushort.MaxValue))
                        {
                            success = false;
                            return default(ushort);
                        }
                        return (ushort) a;
                    }
                    case TypeCode.SByte:
                    {
                        var a = conv.ToSByte(null);
                        if (a < ushort.MinValue)
                        {
                            success = false;
                            return default(ushort);
                        }
                        return (ushort) a;
                    }
                    case TypeCode.Double:
                    {
                        var a = conv.ToDouble(null);
                        if ((a < ushort.MinValue) || (a > ushort.MaxValue))
                        {
                            success = false;
                            return default(ushort);
                        }
                        return (ushort) a;
                    }
                    case TypeCode.Single:
                    {
                        var a = conv.ToSingle(null);
                        if ((a < ushort.MinValue) || (a > ushort.MaxValue))
                        {
                            success = false;
                            return default(ushort);
                        }
                        return (ushort) a;
                    }
                    case TypeCode.UInt16:
                        return conv.ToUInt16(null);
                    case TypeCode.UInt32:
                    {
                        var a = conv.ToUInt32(null);
                        if (a > ushort.MaxValue)
                        {
                            success = false;
                            return default(ushort);
                        }
                        return (ushort) a;
                    }
                    case TypeCode.UInt64:
                    {
                        var a = conv.ToUInt64(null);
                        if (a > ushort.MaxValue)
                        {
                            success = false;
                            return default(ushort);
                        }
                        return (ushort) a;
                    }
                    case TypeCode.Decimal:
                    {
                        var a = conv.ToDecimal(null);
                        if ((a < ushort.MinValue) || (a > ushort.MaxValue))
                        {
                            success = false;
                            return default(ushort);
                        }
                        return (ushort) a;
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
                    success = true;
                    return BitConverter.ToUInt16(bs, 0);
                }
            }
            success = false;
            return default(ushort);
        }
    }
}