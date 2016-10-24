using System;

namespace blqw.Converts
{
    /// <summary>
    /// 布尔值转换器
    /// </summary>
    public class CBoolean : SystemTypeConvertor<bool>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override bool ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            success = true;
            input = input?.Trim() ?? "";
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
                            return true;
                        case '0':
                        case 'F':
                        case 'f':
                        case '错':
                        case '錯':
                        case '假':
                        case '否':
                        case '女':
                            return false;
                        default:
                            break;
                    }
                    break;
                case 2:
                    if ((input[0] == '-') || (input[0] == '+'))
                    {
                        return input[1] != '0';
                    }
                    break;
                case 4:
                    if (input.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    break;
                case 5:
                    if (input.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            success = false;
            return default(bool);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override bool ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null);
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(bool);
                    case TypeCode.Byte:
                        return conv.ToByte(null) != 0;
                    case TypeCode.Char:
                        return conv.ToChar(null) != 0;
                    case TypeCode.Int16:
                        return conv.ToInt16(null) != 0;
                    case TypeCode.Int32:
                        return conv.ToInt32(null) != 0;
                    case TypeCode.Int64:
                        return conv.ToInt64(null) != 0;
                    case TypeCode.SByte:
                        return conv.ToSByte(null) != 0;
                    case TypeCode.Double:
                        return Math.Abs(conv.ToDouble(null)) > 0d;
                    case TypeCode.Single:
                        return Math.Abs(conv.ToSingle(null)) > 0f;
                    case TypeCode.UInt16:
                        return conv.ToUInt16(null) != 0;
                    case TypeCode.UInt32:
                        return conv.ToUInt32(null) != 0;
                    case TypeCode.UInt64:
                        return conv.ToUInt64(null) != 0;
                    case TypeCode.Decimal:
                        return conv.ToDecimal(null) != 0;
                    case TypeCode.Object:
                    case TypeCode.String:
                    default:
                        break;
                }
            }
            success = false;
            return default(bool);
        }
    }
}