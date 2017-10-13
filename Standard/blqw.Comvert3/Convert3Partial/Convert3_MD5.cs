using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace blqw
{
    public partial class Convert3
    {

        /// <summary>
        /// 使用MD5加密
        /// </summary>
        /// <param name="input"> 加密字符串 </param>
        /// <remarks> 周子鉴 2015.08.26 </remarks>
        public static Guid ToMD5_Fast(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Guid.Empty;
            }
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = md5Provider.ComputeHash(bytes);
                Swap(hash, 0, 3); //交换0,3的值
                Swap(hash, 1, 2); //交换1,2的值
                Swap(hash, 4, 5); //交换4,5的值
                Swap(hash, 6, 7); //交换6,7的值
                return new Guid(hash);
            }
        }

        private static void Swap(byte[] arr, int a, int b)
        {
            var temp = arr[a];
            arr[a] = arr[b];
            arr[b] = temp;
        }

        /// <summary>
        /// 产生一个包含随机'盐'的的MD5
        /// </summary>
        /// <param name="input"> 输入内容 </param>
        /// <returns> </returns>
        /// <remarks> 周子鉴 2015.10.03 </remarks>
        public static Guid ToRandomMD5(string input)
        {
            if (input == null)
            {
                input = "";
            }
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                //获取一个随机数,用于充当 "盐"
                var salt = new object().GetHashCode();
                input += salt;
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = md5Provider.ComputeHash(bytes);
                var saltBytes = BitConverter.GetBytes(salt);
                var index = hash[0] % 12 + 1;
                hash[index] = saltBytes[0];
                hash[index + 1] = saltBytes[1];
                hash[index + 2] = saltBytes[2];
                hash[index + 3] = saltBytes[3];
                return new Guid(hash);
            }
        }

        /// <summary>
        /// 对比使用 ToRandomMD5 产生的MD5和原信息是否匹配
        /// </summary>
        /// <param name="input"> 原信息 </param>
        /// <param name="rmd5"> 随机盐MD5 </param>
        /// <returns> </returns>
        /// <remarks> 周子鉴 2015.10.03 </remarks>
        public static bool EqualsRandomMD5(string input, Guid rmd5)
        {
            if (input == null)
            {
                input = "";
            }
            var arr = rmd5.ToByteArray();
            var index = arr[0] % 12 + 1;
            //将盐取出来
            var salt = BitConverter.ToInt32(arr, index);
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                input += salt;
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = md5Provider.ComputeHash(bytes);
                for (var i = 0; i < 16; i++)
                {
                    if (i == index) //跳过盐的部分
                    {
                        i += 4;
                        continue;
                    }
                    if (hash[i] != arr[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
