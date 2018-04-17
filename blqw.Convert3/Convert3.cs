using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Data;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using blqw.Dynamic;
using blqw.IOC;

namespace blqw
{
    /// <summary>
    /// 超级牛逼的类型转换器v3版
    /// </summary>
    public static partial class Convert3
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        public static object ChangeType(this object input, Type outputType, out bool success)
        {
            using (var context = new ConvertContext())
            {
                var conv = context.Get(outputType);
                return conv.ChangeType(context, input, outputType, out success);
            }
        }

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。转换失败抛出异常
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 要返回的对象的类型 </param>
        public static object ChangeType(this object input, Type outputType)
        {
            using (var context = new ConvertContext())
            {
                var conv = context.Get(outputType);
                bool success;
                var result = conv.ChangeType(context, input, outputType, out success);
                if (success == false)
                {
                    context.ThrowIfHaveError();
                }
                return result;
            }
        }

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。转换失败返回默认值
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 要返回的对象的类型 </param>
        /// <param name="defaultValue"> 转换失败时返回的默认值 </param>
        public static object ChangeType(this object input, Type outputType, object defaultValue)
        {
            using (var context = new ConvertContext())
            {
                var conv = context.Get(outputType);
                bool success;
                var result = conv.ChangeType(context, input, outputType, out success);
                return success ? result : defaultValue;
            }
        }


        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <typeparam name="T"> 换转后的类型 </typeparam>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="success"> 是否成功 </param>
        public static T To<T>(this object input, out bool success)
        {
            using (var context = new ConvertContext())
            {
                var conv = context.Get<T>();
                return conv.ChangeType(context, input, typeof(T), out success);
            }
        }

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。转换失败抛出异常
        /// </summary>
        /// <typeparam name="T"> 要返回的对象类型的泛型 </typeparam>
        /// <param name="input"> 需要转换类型的对象 </param>
        public static T To<T>(this object input)
        {
            using (var context = new ConvertContext())
            {
                var conv = context.Get<T>();
                bool success;
                var result = conv.ChangeType(context, input, typeof(T), out success);
                if (success == false)
                {
                    context.ThrowIfHaveError();
                }
                return result;
            }
        }

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。转换失败返回默认值
        /// </summary>
        /// <typeparam name="T"> 要返回的对象类型的泛型 </typeparam>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="defaultValue"> 转换失败时返回的默认值 </param>
        public static T To<T>(this object input, T defaultValue)
        {
            using (var context = new ConvertContext())
            {
                var conv = context.Get<T>();
                bool success;
                var result = conv.ChangeType(context, input, typeof(T), out success);
                return success ? result : defaultValue;
            }
        }

        /// <summary>
        /// 尝试对指定对象进行类型转换,返回是否转换成功
        /// </summary>
        /// <param name="input">需要转换类型的对象</param>
        /// <param name="outputType">转换后的类型</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryChangedType(object input, Type outputType, out object result)
        {
            using (var context = new ConvertContext())
            {
                bool b;
                var conv = context.Get(outputType);
                result = conv.ChangeType(context, input, outputType, out b);
                return b;
            }
        }


        /// <summary>
        /// 转为动态类型
        /// </summary>
        public static dynamic ToDynamic(this object obj)
        {
            if (obj.IsNull())
            {
                return new DynamicPrimitive(null);
            }
            if (obj is IDynamicMetaObjectProvider)
            {
                return obj;
            }

            var str = obj as string;
            if (str != null)
            {
                return new DynamicPrimitive(str);
            }
            var row = obj as DataRow;
            if (row != null)
            {
                return new DynamicDataRow(row);
            }
            var view = obj as DataRowView;
            if (view != null)
            {
                return new DynamicDataRow(view);
            }
            var nv = obj as NameValueCollection;
            if (nv != null)
            {
                return new DynamicNameValueCollection(nv);
            }
            var reader = obj as IDataReader;
            if (reader != null)
            {
                return new DynamicDictionary(reader.To<IDictionary>());
            }

            var dict = obj as IDictionary;
            if (dict != null)
            {
                return new DynamicDictionary(dict);
            }

            var list = obj as IList;
            if (list != null)
            {
                return new DynamicList(list);
            }

            var ee = obj as IDictionaryEnumerator ?? obj as IEnumerator ?? (obj as IEnumerable)?.GetEnumerator();
            if (ee != null)
            {
                return new DynamicEnumerator(ee);
            }

            if ("System".Equals(obj.GetType().Namespace, StringComparison.Ordinal))
            {
                return new DynamicPrimitive(obj);
            }
            return new DynamicEntity(obj);
        }

        /// <summary>
        /// 获取一个类型的默认值
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        [Export("GetDefaultValue")]
        [ExportMetadata("Priority", ExportComponent.PRIORITY)]
        public static object GetDefaultValue(this Type type)
        {
            if ((type == null)
                || (type.IsValueType == false) //不是值类型
                || type.IsGenericTypeDefinition //泛型定义类型
                || (Nullable.GetUnderlyingType(type) != null)) //可空值类型
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        #region 半角全角转换

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

        #endregion

        #region MD5

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
                var index = hash[0]%12 + 1;
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
            var index = arr[0]%12 + 1;
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

        #endregion

        #region Decode

        /// <summary>
        /// 使用16位MD5加密
        /// </summary>
        /// <param name="input"> 加密字符串 </param>
        /// <param name="count"> 加密次数 </param>
        public static string ToMD5x16(string input, int count = 1)
        {
            if (string.IsNullOrEmpty(input))
            {
                return _EmptyMd5x16;
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
            => string.IsNullOrEmpty(input) ? _EmptyMd5x16 : ToMD5x16(Encoding.UTF8.GetBytes(input));

        /// <summary>
        /// 使用MD5加密
        /// </summary>
        /// <param name="input"> 需要加密的字节 </param>
        public static string ToMD5x16(byte[] input)
        {
            if ((input == null) || (input.Length == 0))
            {
                return _EmptyMd5;
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
                return _EmptyMd5;
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

        private static readonly string _EmptyMd5 = new string('0', 32);
        private static readonly string _EmptyMd5x16 = new string('0', 16);

        /// <summary>
        /// 使用MD5加密
        /// </summary>
        /// <param name="input"> 需要加密的字节 </param>
        public static string ToMD5(byte[] input)
        {
            if (input == null)
            {
                return _EmptyMd5;
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

        #endregion

        #region DbType

        /// <summary>
        /// 将 <seealso cref="Type" /> 对象转为对应 <seealso cref="DbType" /> 对象
        /// </summary>
        public static DbType TypeToDbType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    return DbType.Boolean;
                case TypeCode.DBNull:
                    return DbType.Object;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Empty:
                    return DbType.Object;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                case TypeCode.Object:
                default:
                    break;
            }
            if (type == typeof(Guid))
            {
                return DbType.Guid;
            }
            if (type == typeof(byte[]))
            {
                return DbType.Binary;
            }
            if (type == typeof(XmlDocument))
            {
                return DbType.Xml;
            }
            return DbType.Object;
        }

        /// <summary>
        /// 将 <seealso cref="DbType" /> 对象转为对应 <seealso cref="Type" /> 对象
        /// </summary>
        public static Type DbTypeToType(DbType dbtype)
        {
            switch (dbtype)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return typeof(string);
                case DbType.Binary:
                    return typeof(byte[]);
                case DbType.Boolean:
                    return typeof(bool);
                case DbType.Byte:
                    return typeof(byte);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return typeof(DateTime);
                case DbType.Decimal:
                case DbType.VarNumeric:
                case DbType.Currency:
                    return typeof(decimal);
                case DbType.Double:
                    return typeof(double);
                case DbType.Guid:
                    return typeof(Guid);
                case DbType.Int16:
                    return typeof(short);
                case DbType.Int32:
                    return typeof(int);
                case DbType.Int64:
                    return typeof(long);
                case DbType.Object:
                    return typeof(object);
                case DbType.SByte:
                    return typeof(sbyte);
                case DbType.Single:
                    return typeof(float);
                case DbType.UInt16:
                    return typeof(ushort);
                case DbType.UInt32:
                    return typeof(uint);
                case DbType.UInt64:
                    return typeof(ulong);
                case DbType.Xml:
                    return typeof(XmlDocument);
                default:
                    throw new InvalidCastException("无效的DbType值:" + dbtype);
            }
        }

        #endregion

        public static bool IsNull(this object value) =>
            value is null ||
            value is DBNull ||
            value.Equals(null) ||
            value.Equals(DBNull.Value) ||
            (value is IConvertible c && (c.GetTypeCode() == TypeCode.DBNull || c.GetTypeCode() == TypeCode.Empty));
    }
}