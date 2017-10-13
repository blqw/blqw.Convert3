using System;
using System.Globalization;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="CDateTime" /> 转换器
    /// </summary>
    public class CDateTime : SystemTypeConvertor<DateTime>
    {
        /// <summary>
        /// 日期格式化字符
        /// </summary>
        private static readonly string[] _Formats = { "yyyyMMddHHmmss", "yyyyMMddHHmmssfff" };

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override DateTime ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.DateTime:
                        success = true;
                        return conv.ToDateTime(null);
                    case TypeCode.Byte:
                    case TypeCode.Boolean:
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
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
                    case TypeCode.Decimal:
                        success = false;
                        return default(DateTime);
                    case TypeCode.Object:
                        break;
                    case TypeCode.String:
                        return ChangeType(context, conv.ToString(null), outputType, out success);
                    default:
                        break;
                }
            }
            success = false;
            return default(DateTime);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override DateTime ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            DateTime result;
            if (DateTime.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (DateTime.TryParseExact(input, _Formats, null, DateTimeStyles.None, out result))
            {
                success = true;
                return result;
            }
            success = false;
            return default(DateTime);
        }
    }
}