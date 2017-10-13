using System;
using System.Globalization;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="float" /> 转换器
    /// </summary>
    public class CSingle : SystemTypeConvertor<float>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override float ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            float result;
            if (float.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = float.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
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
        protected override float ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? 1 : 0;

                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(float);
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
                        if ((a < float.MinValue) || (a > float.MaxValue))
                        {
                            success = false;
                            return default(float);
                        }
                        return (float) a;
                    }
                    case TypeCode.Single:
                        return conv.ToSingle(null);
                    case TypeCode.UInt16:
                        return conv.ToUInt16(null);
                    case TypeCode.UInt32:
                        return conv.ToUInt32(null);
                    case TypeCode.UInt64:
                        return conv.ToUInt64(null);
                    case TypeCode.Decimal:
                        return (float) conv.ToDecimal(null);
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
                if (bs?.Length == 4)
                {
                    success = true;
                    return BitConverter.ToSingle(bs, 0);
                }
            }
            success = false;
            return default(float);
        }
    }
}