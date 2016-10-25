using System;
using System.Collections.Generic;

namespace blqw
{
    /// <summary>
    /// 异常快照
    /// </summary>
    public struct ExceptionSnapshot
    {
        /// <summary>
        /// 异常栈
        /// </summary>
        private readonly List<Exception> _exceptions;

        /// <summary>
        /// 快照点
        /// </summary>
        private readonly int _snapshot;

        /// <summary>
        /// 初始化异常快照
        /// </summary>
        /// <param name="exceptions"> 异常栈 </param>
        public ExceptionSnapshot(List<Exception> exceptions)
        {
            _exceptions = exceptions;
            _snapshot = exceptions?.Count ?? 0;
        }

        /// <summary>
        /// 恢复异常栈
        /// </summary>
        public void Recovery()
        {
            if (_exceptions == null)
            {
                return;
            }
            var count = _exceptions.Count - _snapshot;
            if (count <= 0)
            {
                return;
            }
            _exceptions.RemoveRange(_snapshot, count);
        }
    }
}