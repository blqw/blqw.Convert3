using System;
using System.Text;

namespace blqw.Converts
{
    internal sealed class CBytes : SystemTypeConvertor<byte[]>
    {
        private static readonly byte[] _Empty = new byte[0];

        protected override byte[] ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            success = true;
            return input.Length == 0 ? _Empty : Encoding.UTF8.GetBytes(input);
        }

        protected override byte[] ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
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
            else if (input is Guid)
            {
                success = true;
                return ((Guid) input).ToByteArray();
            }
            success = false;
            return null;
        }
    }
}