using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CEnum : SystemTypeConvertor<Enum>, IConvertor
    {
        protected override Enum ChangeType(string input, Type outputType, out bool success)
        {
            if (input.Length == 0)
            {
                success = false;
                return null;
            }
            Enum result;
            try
            {
                result = (Enum)Enum.Parse(outputType, input, true);
            }
            catch (Exception ex)
            {
                Error.Add(ex);
                success = false;
                return null;
            }

            if (Enum.IsDefined(outputType, result))
            {
                success = true;
                return result;
            }
            if (Attribute.IsDefined(outputType, typeof(FlagsAttribute)))
            {
                if (result.ToString().Contains(","))
                {
                    success = true;
                    return result;
                }
                Error.CastFail($"{result.ToString()} 不是有效的枚举值");
            }
            success = false;
            return null;
        }

        protected override Enum ChangeType(object input, Type outputType, out bool success)
        {
            success = true;
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                    case TypeCode.Boolean:
                        success = false;
                        return null;
                    case TypeCode.Decimal:
                    case TypeCode.Byte: 
                    case TypeCode.Char: 
                    case TypeCode.Int16: 
                    case TypeCode.Int32: 
                    case TypeCode.Int64: 
                    case TypeCode.SByte: 
                    case TypeCode.Double: 
                    case TypeCode.Single: 
                    case TypeCode.UInt16: 
                    case TypeCode.UInt32: 
                    case TypeCode.UInt64:
                        return (Enum)Enum.ToObject(outputType, conv.ToUInt64(null));
                    default:
                        break;
                }
            }
            success = false;
            return null;
        }

        IConvertor IConvertor.GetConvertor(Type outputType)
        {
            if (outputType == null)
                throw new ArgumentNullException(nameof(outputType));
            if (outputType.IsEnum == false)
                throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}必须是枚举");
            return this;
        }        
    }
}
