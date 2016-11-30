using System;

namespace blqw.Converts
{
    /// <summary>
    /// 父类转换器,提供可生成子类转换的转换器基类
    /// </summary>
    /// <typeparam name="T"> 父类类型 </typeparam>
    /// <remarks>
    /// 高级类型的定义: T 接口,抽象类,或可以被继承的类型,
    /// outputType 不一定完全等于 typeof(T),
    /// 例如:数组,只需要实现Array的转换,并不用实现每一个 T[]
    /// </remarks>
    public abstract class BaseTypeConvertor<T> : BaseConvertor<T>
    {
        /// <summary>
        /// 获取子转换器
        /// </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="outputType" /> is <see langword="null" />. </exception>
        protected override IConvertor GetConvertor(Type outputType)
        {
            if (outputType == null)
            {
                throw new ArgumentNullException(nameof(outputType));
            }
            if (typeof(T) == outputType)
            {
                return this;
            }
            if (OutputType.IsAssignableFrom(outputType) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}不是{OutputType}的子类或实现类");
            }

            var type = typeof(InnerConvertor<>).MakeGenericType(typeof(T), outputType);
            return (IConvertor) Activator.CreateInstance(type, this);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected sealed override T ChangeType(ConvertContext context, object input, Type outputType, out bool success)
        {
            if (outputType.IsGenericTypeDefinition)
            {
                context.AddException("无法转为泛型定义类");
                success = false;
                return default(T);
            }
            return ChangeTypeImpl(context, input, outputType, out success);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected abstract T ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success);

        /// <summary>
        /// 子转换器
        /// </summary>
        /// <typeparam name="TOutput"> 输出类型 </typeparam>
        private class InnerConvertor<TOutput> : IConvertor<TOutput>
            where TOutput : T
        {
            /// <summary>
            /// 转换器服务提供程序，一般为父转换器
            /// </summary>
            private readonly IServiceProvider _provider;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="provider"> 转换器服务提供程序，一般为父转换器 </param>
            public InnerConvertor(IServiceProvider provider)
            {
                _provider = provider;
            }

            /// <summary>
            /// 转换器的输出类型
            /// </summary>
            public Type OutputType => typeof(TOutput);

            /// <summary>
            /// 返回指定类型的对象，其值等效于指定字符串。
            /// </summary>
            /// <param name="context"> 上下文 </param>
            /// <param name="input"> 需要转换类型的字符串对象 </param>
            /// <param name="outputType"> 换转后的类型 </param>
            /// <param name="success"> 是否成功 </param>
            TOutput IConvertor<TOutput>.ChangeType(ConvertContext context, string input, Type outputType, out bool success)
                => (TOutput) context.Get<T>().ChangeType(context, input, outputType, out success);

            /// <summary>
            /// 返回指定类型的对象，其值等效于指定对象。
            /// </summary>
            /// <param name="context"> 上下文 </param>
            /// <param name="input"> 需要转换类型的对象 </param>
            /// <param name="outputType"> 换转后的类型 </param>
            /// <param name="success"> 是否成功 </param>
            TOutput IConvertor<TOutput>.ChangeType(ConvertContext context, object input, Type outputType, out bool success)
                => (TOutput) context.Get<T>().ChangeType(context, input, outputType, out success);

            /// <summary>
            /// 返回指定类型的对象，其值等效于指定字符串对象。
            /// </summary>
            /// <param name="context"> 上下文 </param>
            /// <param name="input"> 需要转换类型的字符串对象 </param>
            /// <param name="outputType"> 换转后的类型 </param>
            /// <param name="success"> 是否成功 </param>
            object IConvertor.ChangeType(ConvertContext context, string input, Type outputType, out bool success)
                => (TOutput) context.Get<T>().ChangeType(context, input, outputType, out success);

            /// <summary>
            /// 返回指定类型的对象，其值等效于指定对象。
            /// </summary>
            /// <param name="context"> 上下文 </param>
            /// <param name="input"> 需要转换类型的对象 </param>
            /// <param name="outputType"> 换转后的类型 </param>
            /// <param name="success"> 是否成功 </param>
            object IConvertor.ChangeType(ConvertContext context, object input, Type outputType, out bool success)
                => (TOutput) context.Get<T>().ChangeType(context, input, outputType, out success);

            /// <summary>
            /// 获取指定类型的服务对象。 
            /// </summary>
            /// <returns>
            /// <paramref name="serviceType" /> 类型的服务对象。- 或 -如果没有 <paramref name="serviceType" /> 类型的服务对象，则为 null。
            /// </returns>
            /// <param name="serviceType"> 一个对象，它指定要获取的服务对象的类型。 </param>
            public object GetService(Type serviceType) => _provider.GetService(serviceType);
        }
    }
}