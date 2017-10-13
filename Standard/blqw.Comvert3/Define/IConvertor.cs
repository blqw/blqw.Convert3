using blqw.Autofac;
using System;

namespace blqw
{
    /// <summary>
    /// 转换器
    /// </summary>
    [InheritedExport(typeof(IConvertor))]
    public interface IConvertor : IServiceProvider
    {
        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        Type OutputType { get; }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        object ChangeType(IConvertContext context, object input, Type outputType, out bool success);

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        object ChangeType(IConvertContext context, string input, Type outputType, out bool success);
    }

    /// <summary>
    /// 泛型转换器
    /// </summary>
    /// <typeparam name="T"> 输出类型泛型 </typeparam>
    public interface IConvertor<out T> : IConvertor
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        new T ChangeType(IConvertContext context, object input, Type outputType, out bool success);

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串。
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        new T ChangeType(IConvertContext context, string input, Type outputType, out bool success);
    }
}