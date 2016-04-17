using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary>
    /// 异常栈
    /// </summary>
    static class Error
    {
        /// <summary>
        /// 当前线程中的异常契约
        /// </summary>
        [ThreadStatic]
        static ErrorContract _Contract;

        /// <summary>
        /// 开始异常契约
        /// </summary>
        /// <returns></returns>
        public static IDisposable Contract()
        {
            if (_Contract.Enabled)
            {
                return null;
            }
            _Contract.Enable();
            return _Contract;
        }

        /// <summary>
        /// 开始一个事务操作
        /// </summary>
        public static Transaction BeginTransaction()
        {
            return new Transaction(_Contract.Count);
        }

        /// <summary>
        /// 异常事务
        /// </summary>
        public struct Transaction
        {
            private int _index;

            /// <summary>
            /// 初始化异常事务
            /// </summary>
            /// <param name="index">契约中异常索引的保存点</param>
            public Transaction(int index)
            {
                _index = index;
            }

            /// <summary>
            /// 回滚事务中产生的所有异常
            /// </summary>
            public void Rollback()
            {
                _Contract.RemoveIndexToEnd(_index);
            }
        }

        /// <summary>
        /// 转换器未找到
        /// </summary>
        /// <param name="type"></param>
        public static void ConvertorNotFound(Type type)
        {
            if (_Contract.Enabled == false) return;
            _Contract.Add(new NotImplementedException($"无法获取 {CType.GetFriendlyName(type)} 类型的转换器"));
        }

        /// <summary>
        /// 因为类型原因导致转换失败
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toType"></param>
        public static void CastFail(Type type, Type toType)
        {
            if (_Contract.Enabled == false) return;
            var text = $"类型 {CType.GetFriendlyName(type)} 无法转为 {CType.GetFriendlyName(toType)} 类型对象";
            _Contract.Add(new InvalidCastException(text));
        }

        /// <summary>
        /// 因为值原因导致转换失败
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toType"></param>
        public static void CastFail(object value, Type toType)
        {
            if (_Contract.Enabled == false) return;
            var text = value == null ? "`null`" : value is DBNull ? "`DBNull`" : null;
            if (text == null)
            {
                text = $"类型:{CType.GetFriendlyName(value.GetType())} \n{value.ToString()}";
            }
            var name = CType.GetFriendlyName(toType);
            _Contract.Add(new InvalidCastException(string.Concat(text, " 无法转为 ", name)));
        }

        public static void ThrowIfHaveError()
        {
            if (_Contract.Enabled == false) return;
            _Contract.ThrowError();
        }

        /// <summary>
        /// 异常契约
        /// </summary>
        struct ErrorContract : IDisposable
        {
            /// <summary>
            /// 异常栈
            /// </summary>
            List<Exception> _Errors;
            /// <summary>
            /// 是否启用
            /// </summary>
            public bool Enabled { get; private set; }

            /// <summary>
            /// 开启契约
            /// </summary>
            public void Enable()
            {
                if (_Errors == null)
                {
                    _Errors = new List<Exception>();
                }
                else if (_Errors.Count > 0)
                {
                    _Errors.Clear();
                }
                Enabled = true;
            }

            /// <summary>
            /// 当前契约中的异常数量
            /// </summary>
            public int Count { get { return _Errors?.Count ?? 0; } }

            /// <summary>
            /// 向当前契约中添加新的异常,如果契约未启用则不执行任何操作
            /// </summary>
            /// <param name="ex">需要添加到契约的异常</param>
            public void Add(Exception ex)
            {
                if (Enabled) _Errors?.Add(ex);
            }

            /// <summary>
            /// 关闭当前契约,销毁所有的异常信息
            /// </summary>
            public void Dispose()
            {
                _Errors?.Clear();
                Enabled = false;
            }

            /// <summary>
            /// 移除从指定索引处(包含)开始的所有异常
            /// </summary>
            /// <param name="index">需要移除异常的开始索引(包含)</param>
            public void RemoveIndexToEnd(int index)
            {
                if (index <= 0 || _Errors == null) return;
                var count = _Errors.Count - index;
                if (count <= 0) return;
                _Errors.RemoveRange(index, count);
            }

            /// <summary>
            /// 抛出当前契约中的所有异常
            /// </summary>
            public void ThrowError()
            {
                if (Enabled == false || _Errors == null || _Errors.Count == 0)
                {
                    return;
                }
                if (_Errors.Count == 1)
                {
                    throw _Errors[0];
                }
                throw new AggregateException(_Errors.Reverse<Exception>());
            }
        }
    }
}
