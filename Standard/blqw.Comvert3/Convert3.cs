using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace blqw
{
    /// <summary>
    /// 超级牛逼的类型转换器v3版
    /// </summary>
    public static partial class Convert3
    {
        public const int PRIORITY = 108;

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        public static object ChangeType(this object input, Type outputType, out bool success) => throw new NotImplementedException();

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。转换失败抛出异常
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 要返回的对象的类型 </param>
        public static object ChangeType(this object input, Type outputType) => throw new NotImplementedException();

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。转换失败返回默认值
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 要返回的对象的类型 </param>
        /// <param name="defaultValue"> 转换失败时返回的默认值 </param>
        public static object ChangeType(this object input, Type outputType, object defaultValue) => throw new NotImplementedException();


        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <typeparam name="T"> 换转后的类型 </typeparam>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="success"> 是否成功 </param>
        public static T To<T>(this object input, out bool success) => throw new NotImplementedException();

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。转换失败抛出异常
        /// </summary>
        /// <typeparam name="T"> 要返回的对象类型的泛型 </typeparam>
        /// <param name="input"> 需要转换类型的对象 </param>
        public static T To<T>(this object input) => throw new NotImplementedException();

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。转换失败返回默认值
        /// </summary>
        /// <typeparam name="T"> 要返回的对象类型的泛型 </typeparam>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="defaultValue"> 转换失败时返回的默认值 </param>
        public static T To<T>(this object input, T defaultValue) => throw new NotImplementedException();

        /// <summary>
        /// 尝试对指定对象进行类型转换,返回是否转换成功
        /// </summary>
        /// <param name="input">需要转换类型的对象</param>
        /// <param name="outputType">转换后的类型</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryChangedType(object input, Type outputType, out object result) => throw new NotImplementedException();


        /// <summary>
        /// 转为动态类型
        /// </summary>
        public static dynamic ToDynamic(this object obj) => throw new NotImplementedException();
    }
}