using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CBytes : SystemTypeConvertor<byte[]>
    {
        protected override byte[] ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            success = true;
            if (input.Length == 0)
            {
                return new byte[0];
            }
            return Encoding.UTF8.GetBytes(input);
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
                    case TypeCode.Decimal:
                        success = false;
                        return null;
                    case TypeCode.Boolean: return BitConverter.GetBytes(conv.ToByte(null)); 
                    case TypeCode.Byte: return BitConverter.GetBytes(conv.ToByte(null)); 
                    case TypeCode.Char: return BitConverter.GetBytes(conv.ToChar(null));
                    case TypeCode.Int16: return BitConverter.GetBytes(conv.ToInt16(null));
                    case TypeCode.Int32: return BitConverter.GetBytes(conv.ToInt32(null));
                    case TypeCode.Int64: return BitConverter.GetBytes(conv.ToInt64(null));
                    case TypeCode.SByte:
                        success = false;
                        return null;
                    case TypeCode.Double: return BitConverter.GetBytes(conv.ToDouble(null));
                    case TypeCode.Single: return BitConverter.GetBytes(conv.ToSingle(null));
                    case TypeCode.UInt16: return BitConverter.GetBytes(conv.ToUInt16(null));
                    case TypeCode.UInt32: return BitConverter.GetBytes(conv.ToUInt32(null));
                    case TypeCode.UInt64: return BitConverter.GetBytes(conv.ToUInt64(null));
                    default:
                        break;
                }
            }
            success = false;
            return null;
        }
        
    }
}
