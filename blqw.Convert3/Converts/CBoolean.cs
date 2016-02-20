using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CBoolean : SystemTypeConvertor<bool>
    {

        protected override bool Try(object input, out bool result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null);
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = false;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null) != 0; return true;
                    case TypeCode.Char: result = conv.ToChar(null) != 0; return true;
                    case TypeCode.Int16: result = conv.ToInt16(null) != 0; return true;
                    case TypeCode.Int32: result = conv.ToInt32(null) != 0; return true;
                    case TypeCode.Int64: result = conv.ToInt64(null) != 0; return true;
                    case TypeCode.SByte: result = conv.ToSByte(null) != 0; return true;
                    case TypeCode.Double: result = conv.ToDouble(null) != 0; return true;
                    case TypeCode.Single: result = conv.ToSingle(null) != 0; return true;
                    case TypeCode.UInt16: result = conv.ToUInt16(null) != 0; return true;
                    case TypeCode.UInt32: result = conv.ToUInt32(null) != 0; return true;
                    case TypeCode.UInt64: result = conv.ToUInt64(null) != 0; return true;
                    case TypeCode.Decimal: result = conv.ToDecimal(null) != 0; return true;
                    default:
                        break;
                }
            }
            result = false;
            return false;
        }

        protected override bool Try(string input, out bool result)
        {
            if (input == null)
            {
                result = false;
                return false;
            }
            switch (input.Length)
            {
                case 1:
                    switch (input[0])
                    {
                        case '1':
                        case 'T':
                        case 't':
                        case '对':
                        case '對':
                        case '真':
                        case '是':
                        case '男':
                            result = true;
                            return true;
                        case '0':
                        case 'F':
                        case 'f':
                        case '错':
                        case '錯':
                        case '假':
                        case '否':
                        case '女':
                            result = false;
                            return true;
                        default:
                            break;
                    }
                    break;
                case 2:
                    if (input[0] == '-' && input[1] == '1')
                    {
                        result = true;
                        return true;
                    }
                    break;
                case 4:
                    if (input.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        result = true;
                        return true;
                    }
                    break;
                case 5:
                    if (input.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        result = false;
                        return true;
                    }
                    break;
                default:
                    break;
            }

            result = false;
            return false;
        }
    }
}
