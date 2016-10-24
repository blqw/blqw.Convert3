using System;
using System.Collections;
using System.Reflection;

namespace blqw.Mapping
{
    /// <summary>
    /// 拥有 Key-Value 或 Name-Value 结构的集合的枚举器
    /// </summary>
    internal struct PairEnumerator : IKeyValueEnumerator
    {
        private const BindingFlags FLAGS = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;

        private readonly IEnumerator _enumerator;
        private readonly Func<object, object> _getKey;
        private readonly Func<object, object> _getValue;
        private bool _first;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="enumerator"></param>
        public PairEnumerator(IEnumerator enumerator)
        {
            Error = null;
            _getKey = null;
            _getValue = null;
            _enumerator = enumerator;
            _first = false;
            while (_enumerator.MoveNext())
            {
                var entry = _enumerator.Current;
                if (entry == null)
                {
                    continue;
                }
                var type = entry.GetType();
                _getKey = type.GetProperty("Key", FLAGS).GetPropertyHandler()?.Get ??
                          type.GetProperty("Name", FLAGS).GetPropertyHandler()?.Get;
                _getValue = type.GetProperty("Value", FLAGS).GetPropertyHandler()?.Get;
                if ((_getKey == null) || (_getValue == null))
                {
                    Error = "值添加到单元格失败:无法获取Key/Name和Value";
                    return;
                }
                _first = true;
            }
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// 将枚举数推进到集合的下一个元素。
        /// </summary>
        /// <returns> 如果枚举数成功地推进到下一个元素，则为 true；如果枚举数越过集合的结尾，则为 false。 </returns>
        /// <exception cref="T:System.InvalidOperationException"> 在创建了枚举数后集合被修改了。 </exception>
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }
            return _enumerator.MoveNext();
        }

        /// <summary>
        /// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"> 在创建了枚举数后集合被修改了。 </exception>
        public void Reset() => _enumerator.Reset();

        /// <summary>
        /// 获取集合中的当前元素的Key
        /// </summary>
        /// <returns> </returns>
        public object GetKey() => _getKey(_enumerator.Current);

        /// <summary>
        /// 获取集合中的当前元素的Value
        /// </summary>
        /// <returns> </returns>
        public object GetValue() => _getValue(_enumerator.Current);
    }
}