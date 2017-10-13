using System;
using System.Text;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="byte" /> 数组转换器
    /// </summary>
    public class CBytes : SystemTypeConvertor<byte[]>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override byte[] ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            success = true;
            return input.Length == 0 ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(input);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override byte[] ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            if (input is IConvertible conv)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return null;
                    case TypeCode.Decimal:
                        var arr = decimal.GetBits(conv.ToDecimal(null));
                        var bytes = new byte[arr.Length << 2];
                        Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
                        return bytes;
                    case TypeCode.Boolean:
                        return BitConverter.GetBytes(conv.ToByte(null));
                    case TypeCode.Byte:
                        return new[] { conv.ToByte(null) };
                    case TypeCode.Char:
                        return BitConverter.GetBytes(conv.ToChar(null));
                    case TypeCode.Int16:
                        return BitConverter.GetBytes(conv.ToInt16(null));
                    case TypeCode.Int32:
                        return BitConverter.GetBytes(conv.ToInt32(null));
                    case TypeCode.Int64:
                        return BitConverter.GetBytes(conv.ToInt64(null));
                    case TypeCode.SByte:
                        success = false;
                        return null;
                    case TypeCode.Double:
                        return BitConverter.GetBytes(conv.ToDouble(null));
                    case TypeCode.Single:
                        return BitConverter.GetBytes(conv.ToSingle(null));
                    case TypeCode.UInt16:
                        return BitConverter.GetBytes(conv.ToUInt16(null));
                    case TypeCode.UInt32:
                        return BitConverter.GetBytes(conv.ToUInt32(null));
                    case TypeCode.UInt64:
                        return BitConverter.GetBytes(conv.ToUInt64(null));
                    case TypeCode.Object:
                        break;
                    case TypeCode.String:
                        return ChangeType(context, conv.ToString(null), outputType, out success);
                    default:
                        break;
                }
            }
            else if (input is Guid g)
            {
                success = true;
                return g.ToByteArray();
            }
            success = false;
            return null;
        }
    }
}