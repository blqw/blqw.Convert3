﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.Converts;

namespace blqw
{
    /// <summary>
    /// 类型转换中所使用的参数
    /// </summary>
    public sealed class ConvertContext : IDisposable
    {
        /// <summary>
        /// 无行为的上下文
        /// </summary>
        public static readonly ConvertContext None = new ConvertContext() { _isNone = true };

        private bool _isNone;

        /// <summary>
        /// 异常栈
        /// </summary>
        private List<Exception> _exceptions;

        /// <summary>
        /// 转换器的额外提供程序
        /// </summary>
        public IServiceProvider ConvertorProvider { get; set; }

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public IConvertor Get(Type outputType)
            => (IConvertor)ConvertorProvider?.GetService(outputType)
            ?? ConvertorServices.Container?.GetConvertor(outputType)
            ?? FailConvertor.NotFound;

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IConvertor<T> Get<T>()
            => (IConvertor<T>)ConvertorProvider?.GetService(typeof(T))
            ?? ConvertorServices.Container?.GetConvertor<T>()
            ?? FailConvertor<T>.NotFound;

        /// <summary>
        /// 添加一个异常到上下文
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public void AddException(string message, Exception innerException = null)
        {
            if (_isNone) return;
            AddException(new InvalidCastException(message, innerException));
        }

        /// <summary>
        /// 添加一个自定义异常到上下文
        /// </summary>
        /// <param name="ex">自定义异常</param>
        public void AddException(Exception ex)
        {
            if (_isNone) return;
            if (_exceptions == null)
            {
                _exceptions = new List<Exception>(20);
            }
            _exceptions.Add(ex);
        }
        /// <summary>
        /// 添加一个转换异常到异常栈
        /// </summary>
        /// <param name="value">需要转换的值</param>
        /// <param name="toType">期望转换的类型</param>
        public void AddCastFailException(object value, Type toType)
        {
            if (_isNone) return;
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
                text = $"{CType.GetFriendlyName(value.GetType())} 值:`{text}`";
            }
            var name = CType.GetFriendlyName(toType);
            AddException(new InvalidCastException($"{text} 无法转为 {name}"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toType"></param>
        public void AddCastFailException(Type type, Type toType)
        {
            if (_isNone) return;
            var text = $"类型 {CType.GetFriendlyName(type)} 无法转为 {CType.GetFriendlyName(toType)} 类型对象";
            AddException(new InvalidCastException(text));
        }

        /// <summary>执行与释放或重置非托管资源关联的应用程序定义的任务。</summary>
        public void Dispose()
        {
            _exceptions?.Clear();
        }

        /// <summary>
        /// 如果有异常则抛出异常
        /// </summary>
        public void ThrowIfHaveError()
        {
            if (_exceptions == null || _exceptions.Count == 0)
            {
                return;
            }
            if (_exceptions.Count == 1)
            {
                throw _exceptions[0];
            }
            throw new AggregateException(_exceptions[0].Message, _exceptions);
        }

        /// <summary>
        /// 建立异常快照
        /// </summary>
        /// <returns></returns>
        public ExceptionSnapshot Snapshot() => new ExceptionSnapshot(_exceptions);
    }
}
