using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    /// <summary> 
    /// 基本转换器,提供基本类型的转换器基类
    /// </summary>
    /// <typeparam name="T">基本类型泛型</typeparam>
    /// <remarks>
    /// 基本类型的定义: T 是密封类或结构体,不会有被继承的情况
    /// </remarks>
    public abstract class BaseConvertor<T> : IConvertor<T>
    {
        private readonly static Type _outputType = typeof(T);

        protected IConvertor<T> This;
        private bool _IsOverrideImpl;

        /// <summary> 转换器优先级,默认0
        /// </summary>
        public virtual uint Priority { get { return 0; } }

        /// <summary> 转换器的输出类型
        /// </summary>
        public virtual Type OutputType { get { return _outputType; } }

        public string OutputTypeName { get; private set; }

        protected virtual bool TryConvertString { get { return false; } }

        void IConvertor.Initialize()
        {
            This = this;
            const BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance;
            _IsOverrideImpl = this.GetType().GetMethod("ChangeTypeImpl", flag).DeclaringType != typeof(BaseConvertor<T>);
            OutputTypeName = CType.GetFriendlyName(OutputType);
            Initialize();
        }
        /// <summary>
        /// 允许子类重写初始化操作
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        public T ChangeType(object input, out bool success)
        {
            return This.ChangeType(input, OutputType, out success);
        }

        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        public T ChangeType(string input, out bool success)
        {
            return This.ChangeType(input, OutputType, out success);
        }


        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        protected abstract T ChangeType(object input, Type outputType, out bool success);

        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        protected abstract T ChangeType(string input, Type outputType, out bool success);

        protected virtual T ChangeTypeImpl(object input, Type outputType, out bool success)
        {
            return ChangeType(input, outputType, out success);
        }


        T IConvertor<T>.ChangeType(object input, Type outputType, out bool success)
        {
            if (input == null || input is DBNull)
            {
                return ChangeType(input, outputType, out success);
            }
            var str = input as string;
            if (str != null)
            {
                return This.ChangeType(str, outputType, out success);
            }
            var contract = Error.Contract();
            success = false;
            try
            {
                //类型相同直接转换
                if (outputType.IsInstanceOfType(input))
                {
                    success = true;
                    return (T)input;
                }

                //子类转换逻辑
                var result = _IsOverrideImpl ?
                    ChangeTypeImpl(input, outputType, out success)
                    : ChangeType(input, outputType, out success);
                if (success)
                {
                    return result;
                }
                if (TryConvertString)
                {
                    Error.BeginTransaction();
                    //尝试转string后转换
                    str = input.ToString();
                    Error.Add(new Exception($"尝试将{input.GetType()}转为字符串 = \"{str}\""));
                    result = This.ChangeType(str, outputType, out success);
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
                if (contract.Enabled) contract.Dispose();
            }

        }

        T IConvertor<T>.ChangeType(string input, Type outputType, out bool success)
        {
            var contract = Error.Contract();
            T result;
            if (input == null)
            {   //是否可以为null
                result = ChangeType((object)null, outputType, out success);
            }
            else
            {
                result = ChangeType(input, outputType, out success);
            }
            if (success)
            {
                Error.Rollback();
            }
            else
            {
                Error.CastFail(input, outputType);
            }
            if (contract.Enabled) contract.Dispose();
            return result;
        }

        object IConvertor.ChangeType(object input, Type outputType, out bool success)
        {
            return This.ChangeType(input, outputType, out success);
        }

        object IConvertor.ChangeType(string input, Type outputType, out bool success)
        {
            return This.ChangeType(input, outputType, out success);
        }

        IConvertor IConvertor.GetConvertor(Type outputType)
        {
            throw new NotSupportedException();
        }
    }
}
