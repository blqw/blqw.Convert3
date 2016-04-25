using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace blqw
{
    partial class Convert3
    {
        #region HiddenMethod 不同解决方案中才有效
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static new bool Equals(object objA, object objB) { return object.Equals(objA, objB); }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static new bool ReferenceEquals(object objA, object objB) { return object.ReferenceEquals(objA, objB); }
        #endregion

        #region 固定参数
        //^[-+]?\d*(?<point>[.])?\d*$
        readonly static Regex CheckNumber = new Regex(@"^[\s\t]*(?<integer>[-+]?\d*)[.]?(?<decimal>\d*[1-9])?[0]*[\s\t]*$", RegexOptions.Compiled);

        readonly static string[] UpperNumbers =
        {
            "零壹貳叁肆伍陸柒捌玖點",
            "零一二三四五六七八九点"
        };
        readonly static string[] NumberUnits =
        {
            "仟萬拾佰億負",
            "千万十百亿负"
        };
        readonly static string[] MoneyUnits =
        {
            "圓角分",
            "元角分"
        };
        
        #endregion

        /// <summary> 将数字文本转换成大写
        /// </summary>
        /// <param name="number">数字文本</param>
        /// <param name="isSimplified">是否只使用简体中文,默认:否</param>
        /// <param name="isMoney">是否是金额模式(忽略小数点后3位,并加上单位"元,角,分或整"),否认:是</param>
        /// <param name="veryBig">是否开启大数字文本模式(接受15位以上整数,及10位以上小数),否认:否</param>
        public static string NumberToUpper(string number, bool isSimplified = false, bool isMoney = true, bool veryBig = false)
        {
            if (number == null)
            {
                throw new ArgumentNullException("number", "number不能为空");
            }
            //number = number.Trim(' ', '\t');    //去掉多余的空格,制表符
            var m = CheckNumber.Match(number);
            if (m.Success == false)
            {
                throw new ArgumentException("number不是数字", "number");
            }

            unsafe
            {
                fixed (char* p = number)
                fixed (char* upnum = UpperNumbers[isSimplified.GetHashCode()])
                fixed (char* numut = NumberUnits[isSimplified.GetHashCode()])
                fixed (char* monut = MoneyUnits[isSimplified.GetHashCode()])
                {
                    var mdec = m.Groups["decimal"];
                    var mInt = m.Groups["integer"];
                    if (mInt.Length > 15 && veryBig == false)
                    {
                        throw new ArgumentException("不建议转换超过15位的整数,除非将veryBig参数设置为true", "number");
                    }
                    if (mdec.Length > 10 && veryBig == false)
                    {
                        throw new ArgumentException("不建议转换超过10位的小,除非将veryBig参数设置为true", "number");
                    }
                    string integer = ParseInteger(p + mInt.Index, p + mInt.Index + mInt.Length - 1, upnum, numut);

                    if (mdec.Success == false)
                    {
                        string unit = null;
                        if (isMoney)
                            unit = monut[0].ToString() + "整";
                        return integer + unit;
                    }
                    else
                    {
                        if (isMoney)
                        {
                            string jiao = upnum[p[mdec.Index] - '0'].ToString();
                            string fen = mdec.Length == 1 ? "0" : upnum[p[mdec.Index + 1] - '0'].ToString();

                            if (jiao != "0")
                            {
                                jiao += monut[1];
                            }

                            if (fen != "0")
                            {
                                jiao += fen + monut[2];
                            }

                            return integer + monut[0].ToString() + jiao;
                        }
                        else
                        {
                            return integer + ParseDecimal(p + mdec.Index, p + mdec.Index + mdec.Length - 1, upnum);
                        }
                    }
                }
            }





        }

        //操作小数部分
        static unsafe string ParseDecimal(char* p, char* end, char* upnum)
        {
            StringBuilder sb = new StringBuilder((int)(end - p));
            sb.Append(upnum[10]);
            while (p <= end)
            {
                sb.Append(upnum[*p - '0']);
                p++;
            }
            return sb.ToString();
        }

        //操作整数部分,为了效率不写递归.....
        static unsafe string ParseInteger(char* p, char* end, char* upnum, char* numut)
        {
            int length = (int)(end - p) + 1;
            StringBuilder sb = new StringBuilder(length * 3);

            if (*p == '-')
            {
                sb.Append(numut[5]);
                p++;
                length--;
                if (*p == '.')
                {
                    sb.Append(upnum[5]);
                }
            }
            else if (*p == '+')
            {
                p++;
                length--;
            }

            bool ling = false;
            bool yi = false;
            bool wan = false;

            while (p <= end)
            {
                int num = *p - '0';         //获得当前的数0-9

                if (num != 0 && ling == true)//需要加 零
                {
                    sb.Append(upnum[0]);
                    ling = false;           //重置 参数
                }

                if (length % 8 == 1)        //判断是否在"亿"位
                {                           //如果是
                    int n = length / 8;     //计算应该有几个"亿"

                    if (num != 0 || yi == true) //判断是否需要加 单位
                    {
                        if (num != 0)               //如果不为 0
                        {
                            sb.Append(upnum[num]);  //加入字符串
                        }
                        if (n > 0) sb.Append(numut[4], n);
                        if (ling) ling = false;     //重置 参数
                        yi = false;                 //重置 参数
                        if (wan) wan = false;       //重置 参数
                    }
                }
                else                                //十千百万
                {
                    var uIndex = length % 4;        //单位索引
                    if (uIndex == 1)                //判断是否在"万"位
                    {
                        if (num != 0 || wan == true)    //判断是否需要加 单位
                        {
                            if (num != 0)               //如果不为 0
                            {
                                sb.Append(upnum[num]);  //加入字符串
                            }
                            sb.Append(numut[uIndex]);
                            if (ling) ling = false; //重置 参数
                            wan = false;            //重置 参数
                            if (!yi) yi = true;     //设定 参数
                        }
                    }
                    else                            //十千百
                    {
                        if (num != 0)               //如果不为 0
                        {
                            if ((uIndex == 2 && num == 1) == false) //排除 "一十二" 只显示 "十二"
                            {
                                sb.Append(upnum[num]);  //加入字符串
                            }
                            sb.Append(numut[uIndex]);//加入单位
                            if (!yi) yi = true;     //设定 参数
                            if (!wan) wan = true;   //设定 参数
                        }
                        else if (ling == false)
                        {
                            ling = true;
                        }
                    }
                }

                length--;
                p++;
            }
            return sb.ToString();
        }

    }
}
