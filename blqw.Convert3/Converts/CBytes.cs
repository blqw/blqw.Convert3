using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CBytes : SystemTypeConvertor<Byte[]>
    {
        protected override bool Try(object input, out byte[] result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                        result = null;
                        return false;
                    case TypeCode.Boolean: result = BitConverter.GetBytes(conv.ToByte(null)); return true;
                    case TypeCode.Byte: result = BitConverter.GetBytes(conv.ToByte(null)); return true;
                    case TypeCode.Char: result = BitConverter.GetBytes(conv.ToChar(null)); return true;
                    case TypeCode.Int16: result = BitConverter.GetBytes(conv.ToInt16(null)); return true;
                    case TypeCode.Int32: result = BitConverter.GetBytes(conv.ToInt32(null)); return true;
                    case TypeCode.Int64: result = BitConverter.GetBytes(conv.ToInt64(null)); return true;
                    case TypeCode.SByte:
                        result = null;
                        return false;
                    case TypeCode.Double: result = BitConverter.GetBytes(conv.ToDouble(null)); return true;
                    case TypeCode.Single: result = BitConverter.GetBytes(conv.ToSingle(null)); return true;
                    case TypeCode.UInt16: result = BitConverter.GetBytes(conv.ToUInt16(null)); return true;
                    case TypeCode.UInt32: result = BitConverter.GetBytes(conv.ToUInt32(null)); return true;
                    case TypeCode.UInt64: result = BitConverter.GetBytes(conv.ToUInt64(null)); return true;
                    default:
                        break;
                }
            }
            result = null;
            return false;
        }


        protected override bool Try(string input, out byte[] result)
        {
            if (input == null)
            {
                result = null;
                return true;
            }
            else if (input.Length == 0)
            {
                result = new byte[0];
                return true;
            }
            result = Encoding.UTF8.GetBytes(input);
            return true;
        }
    }
}
