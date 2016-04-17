using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 
    /// 高级转换器,提供高级类型的转换器基类
    /// <para>高级类型的定义: T 接口,抽象类,或可以被继承的类型,
    /// outputType 不一定完全等于 typeof(T)</para>
    /// </summary>
    /// <typeparam name="T">高级类型泛型</typeparam>
    public abstract class AdvancedConvertor<T> : IConvertor<T>
    {
        private readonly static Type _type = typeof(T);


        /// <summary> 转换器优先级,默认0
        /// </summary>
        public virtual uint Priority { get { return 0; } }

        /// <summary> 转换器的输出类型
        /// (有可能是泛型定义类型)
        /// </summary>
        public Type OutputType { get { return _type; } }

        /// <summary> 尝试转换,返回转换是否成功
        /// </summary>
        /// <param name="input">输入对象</param>
        /// <param name="outputType">输出具体类型</param>
        /// <param name="result">如果转换成功,则包含转换后的对象,否则为default(T)</param>
        protected abstract bool Try(object input, Type outputType, out T result);

        /// <summary> 尝试转换,返回转换是否成功
        /// </summary>
        /// <param name="input">输入对象</param>
        /// <param name="outputType">输出具体类型</param>
        /// <param name="result">如果转换成功,则包含转换后的对象,否则为default(T)</param>
        protected abstract bool Try(string input, Type outputType, out T result);

        #region 实现接口

        bool IConvertor<T>.Try(object input, Type outputType, out T result)
        {
            var str = input as string;
            if (str != null)
            {
                if (outputType == typeof(object))
                {
                    result = (T)input;
                    return true;
                }
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
                if (input is T && outputType.IsInstanceOfType(input))
                {
                    result = (T)input;
                    return true;
                }

                if (Try(input, outputType, out result))
                {
                    ErrorContext.Clear();
                    return true;
                }

                using (ErrorContext.Freeze())
                {
                    if (input == null
                        || Convert3.TryTo(input, out str) == false
                        || Try(str, outputType, out result) == false)
                    {
                        ErrorContext.CastFail(input, outputType);
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
                if (Try(input, outputType, out result))
                {
                    ErrorContext.Clear();
                    return true;
                }
                return false;
            }
        }

        bool IConvertor.Try(object input, Type outputType, out object result)
        {
            var str = input as string;
            if (str != null)
            {
                return ((IConvertor<T>)this).Try(str, outputType, out result);
            }
            using (ErrorContext.Callin())
            {
                if (outputType.IsGenericTypeDefinition)
                {
                    ErrorContext.Error = new InvalidCastException("无法转为泛型定义类");
                    result = null;
                    return false;
                }
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
                T resultT;
                if (input is T && outputType.IsInstanceOfType(input))
                {
                    result = input;
                    ErrorContext.Clear();
                    return true;
                }


                if (Try(input, outputType, out resultT))
                {
                    result = resultT;
                    ErrorContext.Clear();
                    return true;
                }

                ErrorContext.CastFail(input, outputType);

                using (ErrorContext.Freeze())
                {
                    if (input == null
                        || Convert3.TryTo(input, out str) == false
                        || Try(str, outputType, out resultT) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                ErrorContext.Clear();
                result = resultT;
                return true;
            }
        }

        bool IConvertor.Try(string input, Type outputType, out object result)
        {
            using (ErrorContext.Callin())
            {
                if (outputType.IsGenericTypeDefinition)
                {
                    ErrorContext.Error = new InvalidCastException("无法转为泛型定义类");
                    result = null;
                    return false;
                }
                T resultT;
                if (input is T && outputType.IsInstanceOfType(input))
                {
                    result = input;
                    ErrorContext.Clear();
                    return true;
                }

                if (Try(input, outputType, out resultT))
                {
                    result = resultT;
                    ErrorContext.Clear();
                    return true;
                }
                result = null;
                return false;
            }
        }

        #endregion


        void IConvertor.Initialize() { Initialize(); }
        protected virtual void Initialize() { }
    }
}
