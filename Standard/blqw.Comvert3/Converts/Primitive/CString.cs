using System;
using System.Data;
using System.Text;


namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="string" /> 转换器
    /// </summary>
    public class CString : BaseConvertor<string>
    {
        /// <summary>
        /// 返回是否应该尝试转换String后再转换
        /// </summary>
        protected override bool ShouldConvertString => false;

        /// <summary>
        /// 判断是否为16进制格式的字符串,如果为true,将参数s的前缀(0x/&amp;h)去除
        /// </summary>
        /// <param name="s"> 需要判断的字符串 </param>
        /// <returns> </returns>
        public static bool IsHexString(ref string s)
        {
            if ((s == null) || (s.Length == 0))
            {
                return false;
            }
            var c = s[0];
            if (char.IsWhiteSpace(c)) //有空格去空格
            {
                s = s.TrimStart();
            }
            if (s.Length > 2) //判断是否是0x 或者 &h 开头
            {
                switch (c)
                {
                    case '0':
                        switch (s[1])
                        {
                            case 'x':
                            case 'X':
                                s = s.Remove(0, 2);
                                return true;
                            default:
                                return false;
                        }
                    case '&':
                        switch (s[1])
                        {
                            case 'h':
                            case 'H':
                                s = s.Remove(0, 2);
                                return true;
                            default:
                                return false;
                        }
                    default:
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override string ChangeType(IConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            if (input is DataRow || input is DataRowView)
            {
                return ComponentServices.ToJsonString(input);
            }

            var reader = input as IDataReader;
            if (reader != null)
            {
                if (reader.IsClosed)
                {
                    success = false;
                    context.AddException("DataReader已经关闭");
                    return null;
                }
                switch (reader.FieldCount)
                {
                    case 0:
                        return null;
                    case 1:
                        return BaseChangeType(context, reader.GetValue(0), outputType, out success);
                    default:
                        return ComponentServices.ToJsonString(input);
                }
            }

            if ((input == null) || input is DBNull)
            {
                return null;
            }

            if (input is bool)
            {
                return (bool) input ? "true" : "false";
            }

            var convertible = input as IConvertible;
            if (convertible != null)
            {
                return convertible.ToString(null);
            }

            var formattable = input as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(null, null);
            }

            var type = input as Type;
            if (type != null)
            {
                return CType.GetFriendlyName(type);
            }

            var bs = input as byte[];
            if (bs != null)
            {
                return Encoding.UTF8.GetString(bs);
            }

            var ps = input.GetType().GetProperties();
            if (ps.Length > 0)
            {
                return ComponentServices.ToJsonString(input);
            }

            return input.ToString();
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override string ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            success = true;
            return input;
        }
    }
}