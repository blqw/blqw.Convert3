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
    public abstract class BaseTypeConvertor<T> : BaseConvertor<T>, IConvertor
    {
        /// <summary>
        /// 获取子转换器
        /// </summary>
        protected override IConvertor GetConvertor(Type outputType)
        {
            if (outputType == null)
            {
                throw new ArgumentNullException(nameof(outputType));
            }
            if ((OutputType.IsGenericTypeDefinition == false)
                && (OutputType.IsAssignableFrom(outputType) == false))
            {
                throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}不是{OutputType}的子类或实现类");
            }

            var type = typeof(InnerConvertor<>).MakeGenericType(typeof(T), outputType);
            return (IConvertor)Activator.CreateInstance(type);
        }

        protected sealed override T ChangeType(ConvertContext context, object input, Type outputType, out bool success)
        {
            if (outputType.IsGenericTypeDefinition)
            {
                Error.CastFail("无法转为泛型定义类");
                success = false;
                return default(T);
            }
            return ChangeTypeImpl(context, input, outputType, out success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> </param>
        /// <param name="outputType"> </param>
        /// <param name="success"> </param>
        /// <returns> </returns>
        protected abstract T ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success);

        private class InnerConvertor<TOutput> : BaseTypeConvertor<TOutput>
            where TOutput : T
        {

            protected override TOutput ChangeType(ConvertContext context, string input, Type outputType,
                out bool success) => (TOutput)context.Get<T>().ChangeType(context, input, outputType, out success);

            protected override TOutput ChangeTypeImpl(ConvertContext context, object input, Type outputType,
                out bool success) => (TOutput)context.Get<T>().ChangeType(context, input, outputType, out success);

        }
    }
}