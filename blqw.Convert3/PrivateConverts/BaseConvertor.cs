using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 基本转换器,提供基本类型的转换器基类
    /// <para>基本类型的定义: T 是密封类或结构体,不会有被继承的情况</para>
    /// </summary>
    /// <typeparam name="T">基本类型泛型</typeparam>
    public abstract class BaseConvertor<T> : IConvertor<T>
    {
        private readonly static Type _type = typeof(T);

        /// <summary> 转换器优先级,默认0
        /// </summary>
        public virtual uint Priority { get { return 0; } }

        /// <summary> 转换器的输出类型
        /// </summary>
        public Type OutputType { get { return _type; } }

        /// <summary> 尝试转换,返回转换是否成功
        /// </summary>
        /// <param name="input">输入对象</param>
        /// <param name="result">如果转换成功,则包含转换后的对象,否则为default(T)</param>
        protected abstract bool Try(object input, out T result);

        /// <summary> 尝试转换,返回转换是否成功
        /// </summary>
        /// <param name="input">输入对象</param>
        /// <param name="result">如果转换成功,则包含转换后的对象,否则为default(T)</param>
        protected abstract bool Try(string input, out T result);

        #region 实现接口

        bool IConvertor.Try(object input, Type outputType, out object result)
        {
            T r;
            if (((IConvertor<T>)this).Try(input, outputType, out r))
            {
                result = r;
                return true;
            }
            result = null;
            return false;
        }

        bool IConvertor.Try(string input, Type outputType, out object result)
        {
            T r;
            if (((IConvertor<T>)this).Try(input, outputType, out r))
            {
                result = r;
                return true;
            }
            result = null;
            return false;
        }

        bool IConvertor<T>.Try(object input, Type outputType, out T result)
        {
            var str = input as string;
            if (str != null)
            {
                return ((IConvertor<T>)this).Try(str, outputType, out result);
            }
            using (ErrorContext.Callin())
            {
                if (input == null)
                {
                    result = default(T);
                    if (outputType.IsClass
                        || Nullable.GetUnderlyingType(outputType) != null)
                    {
                        return true;
                    }
                    ErrorContext.CastFail(input, outputType);
                    return false;
                }
                if (input is T)
                {
                    result = (T)input;
                    return true;
                }

                if (Try(input, out result))
                {
                    ErrorContext.Clear();
                    return true;
                }

                using (ErrorContext.Freeze())
                {
                    if (input == null
                        || Convert3.TryTo(input, out str) == false
                        || Try(str, out result) == false)
                    {
                        result = default(T);
                        return false;
                    }
                }
                ErrorContext.Clear();
                return true;
            }
        }

        bool IConvertor<T>.Try(string input, Type outputType, out T result)
        {
            using (ErrorContext.Callin())
            {
                if (input == null)
                {
                    result = default(T);
                    if (outputType.IsClass
                        || Nullable.GetUnderlyingType(outputType) != null)
                    {
                        return true;
                    }
                    ErrorContext.CastFail(input, outputType);
                    return false;
                }
                if (Try(input, out result))
                {
                    ErrorContext.Clear();
                    return true;
                }
                return false;
            }
        }
        #endregion


        void IConvertor.Initialize() { Initialize(); }
        protected virtual void Initialize() { }

    }
}
