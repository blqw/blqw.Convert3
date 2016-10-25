using System;
using System.Globalization;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="decimal" /> 转换器
    /// </summary>
    public class CDecimal : SystemTypeConvertor<decimal>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override decimal ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            decimal result;
            if (decimal.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = decimal.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(decimal);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override decimal ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            var conv = input as IConvertible;
            success = true;
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
                        return default(decimal);
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
                        return (decimal) conv.ToDouble(null);
                    case TypeCode.Single:
                        return (decimal) conv.ToSingle(null);
                    case TypeCode.UInt16:
                        return conv.ToUInt16(null);
                    case TypeCode.UInt32:
                        return conv.ToUInt32(null);
                    case TypeCode.UInt64:
                        return conv.ToUInt64(null);
                    case TypeCode.Decimal:
                        return conv.ToDecimal(null);
                    case TypeCode.Object:
                        break;
                    case TypeCode.String:
                        return ChangeType(context, conv.ToString(null), outputType, out success);
                    default:
                        break;
                }
            }
            else if (input is Guid)
            {
                var bytes = ((Guid) input).ToByteArray();
                var arr2 = new int[4];
                Buffer.BlockCopy(bytes, 0, arr2, 0, 16);
                return new decimal(arr2);
            }
            else
            {
                var bytes = input as byte[];
                if (bytes?.Length == 16)
                {
                    var arr2 = new int[4];
                    Buffer.BlockCopy(bytes, 0, arr2, 0, 16);
                    return new decimal(arr2);
                }
            }
            success = false;
            return default(decimal);
        }
    }
}