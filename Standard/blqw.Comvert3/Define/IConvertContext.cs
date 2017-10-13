using System.ComponentModel.Design;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace blqw
{
    /// <summary>
    /// 类型转换中所使用的参数
    /// </summary>
    public interface IConvertContext : IDisposable
    {
        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <param name="outputType"></param>
        /// <returns></returns>
        IConvertor Get(Type outputType);

        /// <summary>
        /// IOC服务
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// 添加一个自定义异常到上下文
        /// </summary>
        /// <param name="ex">自定义异常</param>
        void AddException(Exception ex);

        /// <summary>
        /// 如果有异常则抛出异常
        /// </summary>
        /// <exception cref="AggregateException"> 发生一个或多个转换错误问题 </exception>
        void ThrowIfHasError();

        /// <summary>
        /// 建立异常快照
        /// </summary>
        /// <returns></returns>
        ExceptionSnapshot Snapshot();

        /// <summary>
        /// 成员栈
        /// </summary>
        MemberStack MemberStack { get; }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">参数名</param>
        /// <param name="defaultValue">默认值</param>
        T GetArgument<T>(string name, T defaultValue = default(T));
    }
}
