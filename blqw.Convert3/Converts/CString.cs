using blqw.IOC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CString : SystemTypeConvertor<string>
    {
        /// <summary> 判断是否为16进制格式的字符串,如果为true,将参数s的前缀(0x/&h)去除
        /// </summary>
        /// <param name="s">需要判断的字符串</param>
        /// <returns></returns>
        public static bool IsHexString(ref string s)
        {
            if (s == null || s.Length == 0)
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

        protected override string ChangeTypeImpl(object input, Type outputType, out bool success)
        {
            return ChangeType(input, outputType, out success);
        }

        protected override string ChangeType(object input, Type outputType, out bool success)
        {
            success = true;
            
            if (input == null || input is DBNull)
            {
                return null;
            }

            if (input is bool)
            {
                return (bool)input ? "true" : "false";
            }

            var convertible = input as IConvertible;
            if (convertible != null)
            {
                return convertible.ToString(null);
            }

            var format = input as IFormattable;
            if (format != null)
            {
                return format.ToString(null, null);
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

        protected override string ChangeType(string input, Type outputType, out bool success)
        {
            success = true;
            return input;
        }
    }
}
