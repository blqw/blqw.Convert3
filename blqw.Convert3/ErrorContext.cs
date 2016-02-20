using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary>
    /// 异常上下文
    /// </summary>
    static class ErrorContext
    {
        /// <summary>
        /// 方法调用深度
        /// </summary>
        [ThreadStatic]
        static int _depth;

        /// <summary>
        /// 上下文异常
        /// </summary>
        [ThreadStatic]
        static List<Exception> _errors;

        /// <summary>
        /// 获取上下文中的异常
        /// </summary>
        public static Exception Error
        {
            get
            {
                if (_errors == null || _errors.Count == 0)
                {
                    return null;
                }
                if (_errors.Count == 1)
                {
                    return _errors[0];
                }
                return new AggregateException(_errors.Reverse<Exception>());
            }
            set
            {
                if (value == null || _depth == 0)
                {
                    return;
                }
                if (_errors == null)
                {
                    _errors = new List<Exception>();
                }
                _errors.Add(value);
            }
        }

        /// <summary>
        /// 转换器未找到
        /// </summary>
        /// <param name="type"></param>
        public static void ConvertorNotFound(Type type)
        {
            if (_depth == 0)
            {
                return;
            }
            Error = new NotImplementedException("没有找到指定类型的转换器:" + CType.GetDisplayName(type));
        }

        /// <summary>
        /// 因为类型原因导致转换失败
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toType"></param>
        public static void CastFail(Type type, Type toType)
        {
            if (_depth == 0)
            {
                return;
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            string x = CType.GetDisplayName(type);
            x = "类型 `" + x + "`";
            var name = CType.GetDisplayName(toType);
            Error = new InvalidCastException(string.Concat(x, " 无法转为 ", name));
        }

        /// <summary>
        /// 因为值原因导致转换失败
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toType"></param>
        public static void CastFail(object value, Type toType)
        {
            if (_depth == 0)
            {
                return;
            }
            string x;
            if (value == null)
            {
                x = "值 <null>";
            }
            else if (value is DBNull)
            {
                x = "值 <DBNull>";
            }
            else
            {
                x = CType.GetDisplayName(value.GetType());
                using (Freeze())
                {
                    x = "类型 `" + x + "` 值: " + Convert3.To<string>(value);
                }
            }
            var name = CType.GetDisplayName(toType);
            Error = new InvalidCastException(string.Concat(x, " 无法转为 ", name));
        }

        /// <summary>
        /// 进入方法
        /// </summary>
        /// <returns></returns>
        public static IDisposable Callin()
        {
            return new _Callin(null);
        }

        struct _Callin : IDisposable
        {
            public _Callin(object o)
            {
                _depth++;
            }

            public void Dispose()
            {
                _depth--;
                if (_depth <= 0)
                {
                    _depth = 0;
                    _errors = null;
                }
            }
        }

        /// <summary>
        /// 清除未被冻结的错误
        /// </summary>
        public static void Clear()
        {
            if (_errors == null || _errors.Count == 0)
            {
                return;
            }
            if (_FreezeErrorCount == 0)
            {
                _errors.Clear();
            }
            if (_errors.Count > _FreezeErrorCount)
            {
                _errors.RemoveRange(_FreezeErrorCount, _errors.Count - _FreezeErrorCount);
            }
        }

        /// <summary>
        /// 冻结的错误数量
        /// </summary>
        private static int _FreezeErrorCount;

        /// <summary>
        /// 冻结当前上下文的状态,如果接下载有Clear的操作将保留被冻结之前的数据
        /// </summary>
        /// <returns></returns>
        public static IDisposable Freeze()
        {
            try
            {
                return new _Freeze(_FreezeErrorCount);
            }
            finally
            {
                _FreezeErrorCount = _errors?.Count ?? 0;
            }
        }

        struct _Freeze : IDisposable
        {
            int _errorCount;
            public _Freeze(int errorCount)
            {
                _errorCount = errorCount;
                _isDisposed = false;
            }

            bool _isDisposed;
            public void Dispose()
            {
                if (_isDisposed) return;
                _isDisposed = true;
                if (_FreezeErrorCount > _errorCount)
                {
                    _FreezeErrorCount = _errorCount;
                }
            }
        }
    }
}
