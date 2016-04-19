using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 
    /// 基本转换器,提供基本类型的转换器基类
    /// <para>基本类型的定义: T 是密封类或结构体,不会有被继承的情况</para>
    /// </summary>
    /// <typeparam name="T">基本类型泛型</typeparam>
    public abstract class BaseConvertor<T> : IConvertor<T>
    {
        private readonly static Type _outputType = typeof(T);

        IConvertor<T> This;
        public BaseConvertor()
        {
            This = this;
        }

        /// <summary> 转换器优先级,默认0
        /// </summary>
        public virtual uint Priority { get { return 0; } }

        /// <summary> 转换器的输出类型
        /// </summary>
        public Type OutputType { get { return _outputType; } }
        
        void IConvertor.Initialize() { Initialize(); }
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
        public abstract T ChangeType(object input, Type outputType, out bool success);
        
        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        public abstract T ChangeType(string input, Type outputType, out bool success);

        T IConvertor<T>.ChangeType(object input, Type outputType, out bool success)
        {
            var str = input as string;
            if (str != null || input == null)
            {
                return This.ChangeType(str, outputType, out success);
            }
            var contract = Error.Contract();
            success = false;
            try
            {
                //类型相同直接转换
                if (input is T)
                {
                    success = true;
                    return (T)input;
                }

                //子类转换逻辑
                var result = ChangeType(input, outputType, out success);
                if (success)
                {
                    return result;
                }

                Error.BeginTransaction();
                //尝试转string后转换
                str = Convert3.ChangeType<string>(input, out success);
                if (success)
                {
                    Error.EndTransaction();
                    return This.ChangeType(str, outputType, out success);
                }
                return default(T);
            }
            finally
            {
                if (success)
                {
                    Error.Rollback();
                }
                else
                {
                    Error.CastFail((object)null, outputType);
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
                success = outputType.IsValueType == false
                        || Nullable.GetUnderlyingType(outputType) != null;
                result = default(T);
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
                Error.CastFail((object)null, outputType);
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
    }
}
