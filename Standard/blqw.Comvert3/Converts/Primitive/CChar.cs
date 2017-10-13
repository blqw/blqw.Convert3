using System;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="char" /> 转换器
    /// </summary>
    public class CChar : SystemTypeConvertor<char>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override char ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            if (input?.Length == 1 || (input = input?.Trim()).Length == 1)
            {
                success = true;
                return input[0];
            }
            success = false;
            return default(char);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override char ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            if (input is IConvertible conv)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? 'T' : 'F';
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(char);
                    case TypeCode.Byte:
                        return (char)conv.ToByte(null);
                    case TypeCode.Char:
                        return conv.ToChar(null);
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if ((a < 0) || (a > 255))
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if ((a < 0) || (a > 255))
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if ((a < 0) || (a > 255))
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if ((a < 0) || (a > 255))
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if ((a < 0) || (a > 255))
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if ((a < 0) || (a > 255))
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Object:
                        break;
                    case TypeCode.String:
                        return ChangeType(context, conv.ToString(null), outputType, out success);
                    default:
                        break;
                }
            }
            else if (input is byte[] bs)
            {
                if (bs.Length == 1)
                {
                    success = true;
                    return (char)bs[0];
                }
                if (bs.Length == 2)
                {
                    success = true;
                    return BitConverter.ToChar(bs, 0);
                }
            }
            else if (input is char[] cs && cs.Length == 1)
            {
                success = true;
                return cs[0];
            }
            success = false;
            return default(char);
        }
    }
}