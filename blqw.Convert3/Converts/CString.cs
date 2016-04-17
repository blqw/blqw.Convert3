using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.Convert3Component;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CString : SystemTypeConvertor<string>, IConvertor<string>
    {
        protected override bool Try(object input, out string result)
        {
            var str = input as string;
            if (str != null)
            {
                result = str;
                return true;
            }

            if (input == null || input is DBNull)
            {
                result = null;
                return true;
            }

            if (input is bool)
            {
                result = (bool)input ? "true" : "false";
                return true;
            }

            var convertible = input as IConvertible;
            if (convertible != null)
            {
                result = convertible.ToString(null);
                return true;
            }
            
            var format = input as IFormattable;
            if (format != null)
            {
                result = format.ToString(null, null);
                return true;
            }

            var type = input as Type;
            if (type != null)
            {
                result = CType.GetFriendlyName(type); 
                return true;
            }

            var bs = input as byte[];
            if (bs != null)
            {
                result = Encoding.UTF8.GetString(bs);
                return true;
            }

            var ps = input.GetType().GetProperties();
            if (ps.Length > 0)
            {
                result = Component.ToJsonString(input);
                return true;
            }

            result = input.ToString();
            return true;
        }

        protected override bool Try(string input, out string result)
        {
            result = input;
            return true;
        }

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
        
    }
}
