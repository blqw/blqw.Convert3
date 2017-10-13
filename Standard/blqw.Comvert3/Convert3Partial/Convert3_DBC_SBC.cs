using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    //半角全角转换
    public partial class Convert3
    {
        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="input"> 任意字符串 </param>
        /// <returns> 全角字符串 </returns>
        /// <remarks>
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </remarks>
        public static string ToSBC(string input)
        {
            if (input == null)
            {
                return null;
            }
            //半角转全角：
            var arr = input.ToCharArray();
            var length = arr.Length;
            unsafe
            {
                fixed (char* p = arr)
                {
                    for (var i = 0; i < length; i++)
                    {
                        var c = p[i];
                        if (TryToSBC(ref c))
                        {
                            p[i] = c;
                        }
                    }
                    return new string(p, 0, length);
                }
            }
        }

        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="c"> 字符 </param>
        /// <remarks>
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </remarks>
        public static char ToSBC(char c)
        {
            TryToSBC(ref c);
            return c;
        }

        /// <summary>
        /// 半角转全角,返回是否转换成功
        /// </summary>
        /// <param name="c"> 字符 </param>
        /// <remarks>
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </remarks>
        public static bool TryToSBC(ref char c)
        {
            if (c < 127)
            {
                if (c > 32)
                {
                    c = (char)(c + 65248);
                    return true;
                }
                if (c == 32)
                {
                    c = (char)12288;
                    return true;
                }
            }
            return false;
        }

        /// <summary> 
        /// 全角转半角(DBC case)
        /// </summary>
        /// <param name="input"> 任意字符串 </param>
        /// <returns> 半角字符串 </returns>
        /// <remarks>
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </remarks>
        public static string ToDBC(string input)
        {
            if (input == null)
            {
                return null;
            }
            //半角转全角：
            var arr = input.ToCharArray();
            var length = arr.Length;
            unsafe
            {
                fixed (char* p = arr)
                {
                    for (var i = 0; i < length; i++)
                    {
                        var c = p[i];
                        if (TryToDBC(ref c))
                        {
                            p[i] = c;
                        }
                    }
                    return new string(p, 0, length);
                }
            }
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="c"> 字符 </param>
        /// <remarks>
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </remarks>
        public static char ToDBC(char c)
        {
            TryToDBC(ref c);
            return c;
        }

        /// <summary>
        /// 全角转半角,返回是否转换成功
        /// </summary>
        /// <param name="c"> 字符 </param>
        /// <remarks>
        /// 全角空格为12288，半角空格为32
        /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </remarks>
        public static bool TryToDBC(ref char c)
        {
            if (c == 12288)
            {
                c = (char)32;
                return true;
            }
            if ((c >= 65281) && (c <= 65374))
            {
                c = (char)(c - 65248);
                return true;
            }
            return false;
        }
    }
}
