using System;

namespace blqw.Converts
{
    /// <summary>
    /// 用于描述失败错误的泛型转换器
    /// </summary>
    internal sealed class FailConvertor<T> : IConvertor<T>
    {
        /// <summary>
        /// 未找到转换错误
        /// </summary>
        public static readonly FailConvertor<T> NotFound = new FailConvertor<T>("无法获取指定类型的转换器");

        /// <summary>
        /// 失败消息
        /// </summary>
        private readonly string _message;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="message"> 失败消息 </param>
        public FailConvertor(string message) 
            => _message = message ?? throw new ArgumentNullException(nameof(message));

        /// <summary> 
        /// 获取指定类型的服务对象。 
        /// </summary>
        /// <returns>
        /// <paramref name="serviceType" /> 类型的服务对象。- 或 -如果没有 <paramref name="serviceType" /> 类型的服务对象，则为 null。
        /// </returns>
        /// <param name="serviceType"> 一个对象，它指定要获取的服务对象的类型。 </param>
        public object GetService(Type serviceType) => this;

        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        public Type OutputType => typeof(void);

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        object IConvertor.ChangeType(IConvertContext context, object input, Type outputType, out bool success)
        {
            context.AddException(_message);
            success = false;
            return null;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串。
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        public T ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            context.AddException(_message);
            success = false;
            return default(T);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        public T ChangeType(IConvertContext context, object input, Type outputType, out bool success)
        {
            context.AddException(_message);
            success = false;
            return default(T);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        object IConvertor.ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            context.AddException(_message);
            success = false;
            return null;
        }
    }
}