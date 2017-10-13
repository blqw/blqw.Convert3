using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 保存 MemberInfo 的后进先出栈
    /// </summary>
    public sealed class MemberStack
    {
        public struct EntryToken : IDisposable
        {
            private readonly Stack<MemberInfo> _stack;
            private readonly int _count;
            public EntryToken(Stack<MemberInfo> stack)
            {
                _stack = stack ?? throw new ArgumentNullException(nameof(stack));
                _count = _stack.Count;
            }

            public void Dispose()
            {
                while (_stack.Count > _count)
                {
                    _stack.Pop();
                }
            }
        }

        private Stack<MemberInfo> _stack;

        public MemberInfo Current => _stack?.Count > 0 ? _stack.Peek() : null;

        /// <summary>
        /// 追加到栈结尾
        /// </summary>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public EntryToken Push(MemberInfo member)
        {
            if (_stack == null)
            {
                _stack = new Stack<MemberInfo>();
            }
            _stack.Push(member ?? throw new ArgumentNullException(nameof(member)));
            return new EntryToken(_stack);
        }
    }
}
