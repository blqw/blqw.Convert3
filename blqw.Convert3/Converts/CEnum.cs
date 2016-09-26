using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CEnum : CEnum<int>
    {
        public override Type OutputType
        {
            get
            {
                return typeof(Enum);
            }
        }
    }

    public class CEnum<T> : SystemTypeConvertor<Enum>, IConvertor, IConvertor<T>
         where T : struct
    {
        public override Type OutputType
        {
            get
            {
                return typeof(T);
            }
        }
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

            if (result.Equals(default(T)))
            {
                success = true;
                return result;
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
            }
            Error.CastFail($"{result.ToString()} 不是有效的枚举值");
            success = false;
            return null;
        }

        protected override Enum ChangeType(object input, Type outputType, out bool success)
        {
            success = true;
            Enum result;
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
                    case TypeCode.Char:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        result = (Enum)Enum.ToObject(outputType, conv.ToInt64(null));
                        break;
                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        result = (Enum)Enum.ToObject(outputType, conv.ToUInt64(null));
                        break;
                    default:
                        success = false;
                        return null;
                }
                if (result.Equals(default(T)))
                {
                    success = true;
                    return result;
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
                }
                Error.CastFail($"{result.ToString()} 不是有效的枚举值");
            }
            success = false;
            return null;
        }

        T IConvertor<T>.ChangeType(string input, Type outputType, out bool success)
        {
            if (input.Length == 0)
            {
                success = false;
                return default(T);
            }
            T result;
            if (Enum.TryParse<T>(input, true, out result) == false)
            {
                success = false;
                return default(T);
            }
            if (result.Equals(default(T)))
            {
                success = true;
                return result;
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
            }
            Error.CastFail($"{result.ToString()} 不是有效的枚举值");
            success = false;
            return default(T);
        }

        T IConvertor<T>.ChangeType(object input, Type outputType, out bool success)
        {
            success = true;
            T result;
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
                        return default(T);
                    case TypeCode.Decimal:
                    case TypeCode.Char:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        result = (T)Enum.ToObject(outputType, conv.ToInt64(null));
                        break;
                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        result = (T)Enum.ToObject(outputType, conv.ToUInt64(null));
                        break;
                    default:
                        success = false;
                        return default(T);
                }
                if (result.Equals(default(T)))
                {
                    success = true;
                    return result;
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
                }
                Error.CastFail($"{result.ToString()} 不是有效的枚举值");
            }
            success = false;
            return default(T);
        }

        object IServiceProvider.GetService(Type outputType)
        {
            if (outputType == null)
                throw new ArgumentNullException(nameof(outputType));
            if (outputType.IsEnum == false)
                throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}必须是枚举");
            var type = typeof(CEnum<>).MakeGenericType(outputType);
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }
    }
}
