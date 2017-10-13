using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace blqw
{
    public partial class Convert3
    {

        /// <summary>
        /// 使用16位MD5加密
        /// </summary>
        /// <param name="input"> 加密字符串 </param>
        /// <param name="count"> 加密次数 </param>
        public static string ToMD5x16(string input, int count = 1)
        {
            if (string.IsNullOrEmpty(input))
            {
                return _emptyMd5x16;
            }
            if (count <= 0)
            {
                return input;
            }
            for (var i = 0; i < count; i++)
            {
                input = ToMD5x16(input);
            }
            return input;
        }

        /// <summary>
        /// 使用16位MD5加密
        /// </summary>
        /// <param name="input"> 加密字符串 </param>
        public static string ToMD5x16(string input)
            => string.IsNullOrEmpty(input) ? _emptyMd5x16 : ToMD5x16(Encoding.UTF8.GetBytes(input));

        /// <summary>
        /// 使用MD5加密
        /// </summary>
        /// <param name="input"> 需要加密的字节 </param>
        public static string ToMD5x16(byte[] input)
        {
            if ((input == null) || (input.Length == 0))
            {
                return _emptyMd5;
            }
            var md5 = new MD5CryptoServiceProvider();
            var data = md5.ComputeHash(input);
            return ByteToString(data, 4, 8);
        }

        /// <summary>
        /// 使用MD5加密
        /// </summary>
        /// <param name="input"> 加密字符串 </param>
        /// <param name="count"> 加密次数 </param>
        public static string ToMD5(string input, int count = 1)
        {
            if (input == null)
            {
                return _emptyMd5;
            }
            if (count <= 0)
            {
                return input;
            }
            for (var i = 0; i < count; i++)
            {
                input = ToMD5(input);
            }
            return input;
        }

        /// <summary>
        /// 使用MD5加密
        /// </summary>
        /// <param name="input"> 加密字符串 </param>
        public static string ToMD5(string input) => ToMD5(Encoding.UTF8.GetBytes(input));

        private static readonly string _emptyMd5 = new string('0', 32);
        private static readonly string _emptyMd5x16 = new string('0', 16);

        /// <summary>
        /// 使用MD5加密
        /// </summary>
        /// <param name="input"> 需要加密的字节 </param>
        public static string ToMD5(byte[] input)
        {
            if (input == null)
            {
                return _emptyMd5;
            }
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var data = md5.ComputeHash(input);
                return ByteToString(data);
            }
        }

        /// <summary>
        /// 使用SHA1加密
        /// </summary>
        /// <param name="input"> 加密字符串 </param>
        /// <param name="count"> 加密次数 </param>
        public static string ToSHA1(string input, int count = 1)
        {
            if ((input == null) || (count <= 0))
            {
                return input;
            }
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var data = Encoding.UTF8.GetBytes(input);
                for (var i = 0; i < count; i++)
                {
                    data = sha1.ComputeHash(data);
                }
                return ByteToString(data);
            }
        }

        /// <summary>
        /// 使用SHA1加密
        /// </summary>
        /// <param name="input"> 加密字符串 </param>
        public static string ToSHA1(string input) => input == null ? null : ToSHA1(Encoding.UTF8.GetBytes(input));

        /// <summary>
        /// 使用SHA1加密
        /// </summary>
        /// <param name="input"> 需要加密的字节 </param>
        public static string ToSHA1(byte[] input)
        {
            if (input == null)
            {
                return null;
            }
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var data = sha1.ComputeHash(input);
                return ByteToString(data);
            }
        }

        private static string ByteToString(IReadOnlyList<byte> data) => ByteToString(data, 0, data.Count);

        private static string ByteToString(IReadOnlyList<byte> data, int offset, int count)
        {
            var chArray = new char[count * 2];
            var end = offset + count;
            for (int i = offset, j = 0; i < end; i++)
            {
                var num2 = data[i];
                chArray[j++] = NibbleToHex((byte)(num2 >> 4));
                chArray[j++] = NibbleToHex((byte)(num2 & 15));
            }
            return new string(chArray);
        }

        private static char NibbleToHex(byte nibble)
            => nibble < 10 ? (char)(nibble + 0x30) : (char)(nibble - 10 + 'a');

    }
}
