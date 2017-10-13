using blqw.Converts;
using System;

namespace blqw
{
    /// <summary>
    /// 拓展方法
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// 添加一个转换异常到异常栈
        /// </summary>
        /// <param name="value">需要转换的值</param>
        /// <param name="toType">期望转换的类型</param>
        public static void AddCastFailException(this IConvertContext context, object value, Type toType)
        {
            if (context != null)
            {
                var text = (value is DBNull ? "`DBNull`" : null)
                       ?? (value as IConvertible)?.ToString(null)
                       ?? (value as IFormattable)?.ToString(null, null)
                       ?? (value == null ? "`null`" : null);

                if (text == null)
                {
                    text = CType.GetFriendlyName(value.GetType());
                }
                else
                {
                    text = $"{CType.GetFriendlyName(value?.GetType())} 值:`{text}`";
                }
                var name = CType.GetFriendlyName(toType);
                context.AddException(new InvalidCastException($"{text} 无法转为 {name}"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toType"></param>
        public static void AddCastFailException(this IConvertContext context, Type type, Type toType)
        {
            if (context != null)
            {
                var text = $"类型 {CType.GetFriendlyName(type)} 无法转为 {CType.GetFriendlyName(toType)} 类型对象";
                context.AddException(new InvalidCastException(text));
            }
        }


        /// <summary>
        /// 添加一个异常到上下文
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public static void AddException(this IConvertContext context, string message, Exception innerException = null) 
            => context?.AddException(new InvalidCastException(message, innerException));


        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IConvertor<T> Get<T>(this IConvertContext context)
            => (IConvertor<T>)context.Get(typeof(T));


    }
}
