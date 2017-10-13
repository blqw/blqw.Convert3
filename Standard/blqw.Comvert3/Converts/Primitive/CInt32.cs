using System;
using System.Globalization;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="int" /> 转换器
    /// </summary>
    public class CInt32 : SystemTypeConvertor<int>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override int ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
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

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override int ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
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
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? 1 : 0;
                    case TypeCode.Byte:
                        return conv.ToByte(null);
                    case TypeCode.Char:
                        return conv.ToChar(null);
                    case TypeCode.Int16:
                        return conv.ToInt16(null);
                    case TypeCode.Int32:
                        return conv.ToInt32(null);
                    case TypeCode.Int64:
                    {
                        var a = conv.ToInt64(null);
                        if ((a < -2147483648) || (a > 2147483647))
                        {
                            success = false;
                            return 0;
                        }
                        return (int) a;
                    }
                    case TypeCode.SByte:
                        return conv.ToSByte(null);
                    case TypeCode.Double:
                    {
                        var a = conv.ToDouble(null);
                        if ((a < -2147483648) || (a > 2147483647))
                        {
                            success = false;
                            return 0;
                        }
                        return (int) a;
                    }
                    case TypeCode.Single:
                    {
                        var a = conv.ToSingle(null);
                        if ((a < -2147483648) || (a > 2147483647))
                        {
                            success = false;
                            return 0;
                        }
                        return (int) a;
                    }
                    case TypeCode.UInt16:
                        return conv.ToUInt16(null);
                    case TypeCode.UInt32:
                    {
                        var a = conv.ToUInt32(null);
                        if (a > 2147483647)
                        {
                            success = false;
                            return 0;
                        }
                        return (int) a;
                    }
                    case TypeCode.UInt64:
                    {
                        var a = conv.ToUInt64(null);
                        if (a > 2147483647)
                        {
                            success = false;
                            return 0;
                        }
                        return (int) a;
                    }
                    case TypeCode.Decimal:
                    {
                        var a = conv.ToDecimal(null);
                        if ((a < -2147483648) || (a > 2147483647))
                        {
                            success = false;
                            return 0;
                        }
                        return (int) a;
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
                if ((a < -2147483648) || (a > 2147483647))
                {
                    success = false;
                    return 0;
                }
                success = true;
                return (int) a;
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr) input).ToUInt64();
                if (a > 2147483647)
                {
                    success = false;
                    return 0;
                }
                success = true;
                return (int) a;
            }
            else
            {
                var bs = input as byte[];
                if (bs?.Length == 4)
                {
                    success = true;
                    return BitConverter.ToInt32(bs, 0);
                }
            }
            success = false;
            return 0;
        }
    }
}