using System;

namespace blqw.Converts
{
    /// <summary>
    /// 基本转换器,提供基本类型的转换器基类
    /// </summary>
    /// <typeparam name="T"> 基本类型泛型 </typeparam>
    /// <remarks>
    /// 基本类型的定义: T 是密封类或结构体,不会有被继承的情况
    /// </remarks>
    public abstract class BaseConvertor<T> : IConvertor<T>
    {
        /// <summary>
        /// 返回是否应该尝试转换String后再转换
        /// </summary>
        protected virtual bool ShouldConvertString => false;

        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        public virtual Type OutputType => typeof(T);
        
        T IConvertor<T>.ChangeType(ConvertContext context, object input, Type outputType, out bool success)
            => BaseChangeType(context, input, outputType, out success);

        T IConvertor<T>.ChangeType(ConvertContext context, string input, Type outputType, out bool success)
            => BaseChangeType(context, input, outputType, out success);

        object IConvertor.ChangeType(ConvertContext context, object input, Type outputType, out bool success)
            => BaseChangeType(context, input, outputType, out success);

        object IConvertor.ChangeType(ConvertContext context, string input, Type outputType, out bool success)
            => BaseChangeType(context, input, outputType, out success);


        object IServiceProvider.GetService(Type outputType) => GetConvertor(outputType);


        protected T BaseChangeType(ConvertContext context, object input, Type outputType, out bool success)
        {
            if ((input == null) || input is DBNull)
            {
                return ChangeType(context, input, outputType, out success);
            }
            var str = input as string;
            if (str != null)
            {
                return BaseChangeType(context, str, outputType, out success);
            }
            var contract = Error.Contract();
            success = false;
            try
            {
                //类型相同直接转换
                if (outputType.IsInstanceOfType(input))
                {
                    success = true;
                    return (T) input;
                }

                //子类转换逻辑
                var result = ChangeType(context, input, outputType, out success);
                if (success)
                {
                    return result;
                }
                if (ShouldConvertString)
                {
                    Error.BeginTransaction();
                    //尝试转string后转换
                    str = input.ToString();
                    Error.Add(new Exception($"尝试将{input.GetType()}转为字符串 = \"{str}\""));
                    result = ((IConvertor<T>) this).ChangeType(context, str, outputType, out success);
                    if (success)
                    {
                        Error.Rollback();
                    }
                    else
                    {
                        Error.EndTransaction();
                    }
                    return result;
                }
                else
                {
                    return default(T);
                }
            }
            finally
            {
                if (success == false)
                {
                    Error.CastFail(input, outputType);
                }
                if (contract.Enabled)
                {
                    contract.Dispose();
                }
            }
        }

        protected T BaseChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            var contract = Error.Contract();
            T result;
            if (input == null)
            {
                //是否可以为null
                result = ChangeType(context, (object) null, outputType, out success);
            }
            else
            {
                result = ChangeType(context, input, outputType, out success);
            }
            if (success)
            {
                Error.Rollback();
            }
            else
            {
                Error.CastFail(input, outputType);
            }
            if (contract.Enabled)
            {
                contract.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 获取子转换器
        /// </summary>
        protected virtual IConvertor GetConvertor(Type outputType) => this;

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected abstract T ChangeType(ConvertContext context, object input, Type outputType, out bool success);

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected abstract T ChangeType(ConvertContext context, string input, Type outputType, out bool success);
    }
}