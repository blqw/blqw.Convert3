using blqw.Converts;
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
        public static ErrorContract Contract()
        {
            if (_Contract.Enabled)
            {
                return default(ErrorContract);
            }
            _Contract.Enable();
            return _Contract;
        }


        public static void BeginTransaction() => _Contract.BeginTransaction();

        public static void Rollback() => _Contract.Rollback();

        public static void EndTransaction() => _Contract.Commit();



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
        /// 直接说明转换失败的原因
        /// </summary>
        /// <param name="message">转换失败原因</param>
        public static void CastFail(string message)
        {
            _Contract.Add(new InvalidCastException(message));
        }

        /// <summary>
        /// 因为值原因导致转换失败
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toType"></param>
        public static void CastFail(object value, Type toType)
        {
            if (_Contract.Enabled == false) return;

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
            _Contract.Add(new InvalidCastException(string.Concat(text, " 无法转为 ", name)));
        }

        public static void ThrowIfHaveError()
        {
            if (_Contract.Enabled == false) return;
            _Contract.ThrowError();
        }

        /// <summary>
        /// 将异常添加到异常栈
        /// </summary>
        /// <param name="ex"></param>
        public static void Add(Exception ex)
        {
            _Contract.Add(ex);
        }


        class ExceptionCollection : List<Exception>
        {
            public ExceptionCollection() : base(10) { }
            public int Indent { get; set; }
        }
        /// <summary>
        /// 异常契约
        /// </summary>
        public struct ErrorContract : IDisposable
        {
            /// <summary>
            /// 异常栈
            /// </summary>
            ExceptionCollection _Stack;
            const int INDENT = 4;
            /// <summary>
            /// 是否启用
            /// </summary>
            public bool Enabled { get; private set; }

            /// <summary>
            /// 开启契约
            /// </summary>
            public void Enable()
            {
                if (_Stack == null)
                {
                    _Stack = new ExceptionCollection();
                }
                else if (_Stack.Count > 0)
                {
                    _Stack.Clear();
                }
                Enabled = true;
            }

            /// <summary>
            /// 向当前契约中添加新的异常,如果契约未启用则不执行任何操作
            /// </summary>
            /// <param name="ex">需要添加到契约的异常</param>
            public void Add(Exception ex)
            {
                if (Enabled)
                {
                    ex.Data["Indent"] = _Stack.Indent;
                    _Stack.Add(ex);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void BeginTransaction()
            {
                if (_Stack?.Count > 0)
                {
                    _Stack.Indent += INDENT;
                    _Stack.Add(null);
                }
            }

            public void Rollback()
            {
                if (_Stack?.Count > 0)
                {
                    _Stack.Indent -= INDENT;
                    var index = _Stack.LastIndexOf(null);
                    if (index <= 0)
                    {
                        _Stack.Clear();
                        return;
                    }
                    var count = _Stack.Count - index;
                    if (count <= 0)
                    {
                        return;
                    }
                    _Stack.RemoveRange(index, count);
                }
            }

            public void Commit()
            {
                if (_Stack?.Count > 0)
                {
                    _Stack.Indent -= INDENT;
                    var index = _Stack.LastIndexOf(null);
                    if (index <= 0)
                    {
                        throw new NotSupportedException($"在错误的位置调用{nameof(Commit)}");
                    }
                    _Stack.RemoveAt(index);
                }
            }

            /// <summary>
            /// 关闭当前契约,销毁所有的异常信息
            /// </summary>
            public void Dispose()
            {
                _Stack?.Clear();
                Enabled = false;
            }

            /// <summary>
            /// 抛出当前契约中的所有异常
            /// </summary>
            public void ThrowError()
            {
                if (Enabled == false || _Stack == null || _Stack.Count == 0)
                {
                    return;
                }
                if (_Stack.Count == 1)
                {
                    throw _Stack[0];
                }
                var message = string.Join(Environment.NewLine, _Stack.Select(it => $"{new string(' ', (int)it.Data["Indent"])}{it.Message}"));
                throw new AggregateException(message, _Stack.Reverse<Exception>());
            }
        }

    }
}
