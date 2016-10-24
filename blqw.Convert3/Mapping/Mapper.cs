using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using blqw.Mapping;

namespace blqw
{
    /// <summary>
    /// 用于枚举对象或集合的键值
    /// </summary>
    public struct Mapper : IDictionaryEnumerator
    {
        /// <summary>
        /// 将枚举数推进到集合的下一个元素。
        /// </summary>
        /// <returns> 如果枚举数成功地推进到下一个元素，则为 true；如果枚举数越过集合的结尾，则为 false。 </returns>
        public readonly Func<bool> MoveNext;

        /// <summary>
        /// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
        /// </summary>
        public readonly Action Reset;

        private readonly Func<object> _getValue;
        private readonly Func<object> _getKey;
        private readonly IDictionaryEnumerator _enumerator;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="input"></param>
        public Mapper(object input)
            : this()
        {
            var reader = input as IDataReader;
            if (reader != null)
            {
                var e = new DataReaderEnumerator(reader);
                Error = e.Error;
                MoveNext = e.MoveNext;
                Reset = e.Reset;
                _getValue = e.GetValue;
                _getKey = e.GetKey;
                return;
            }

            var nv = input as NameValueCollection;
            if (nv != null)
            {
                var e = new NameValueEnumerator(nv);
                Error = e.Error;
                MoveNext = e.MoveNext;
                Reset = e.Reset;
                _getValue = e.GetValue;
                _getKey = e.GetKey;
                return;
            }

            var row = (input as DataRowView)?.Row ?? input as DataRow;
            if (row?.Table != null)
            {
                var e = new DataRowEnumerator(row);
                Error = e.Error;
                MoveNext = e.MoveNext;
                Reset = e.Reset;
                _getValue = e.GetValue;
                _getKey = e.GetKey;
                return;
            }

            var dataset = input as DataSet;
            if (dataset != null)
            {
                var e = new DataSetEnumerator(dataset);
                Error = e.Error;
                MoveNext = e.MoveNext;
                Reset = e.Reset;
                _getValue = e.GetValue;
                _getKey = e.GetKey;
                return;
            }

            var dict = input as IDictionary;
            if (dict != null)
            {
                _enumerator = dict.GetEnumerator();
                MoveNext = _enumerator.MoveNext;
                Reset = _enumerator.Reset;
                return;
            }

            var ee = (input as IEnumerable)?.GetEnumerator() ?? input as IEnumerator;
            if (ee != null)
            {
                var e = new PairEnumerator(ee);
                Error = e.Error;
                MoveNext = e.MoveNext;
                Reset = e.Reset;
                _getValue = e.GetValue;
                _getKey = e.GetKey;
                return;
            }

            var ps = PublicPropertyCache.GetByType(input.GetType());
            if (ps.Length > 0)
            {
                var e = new PropertyEnumerator(input, ps);
                Error = e.Error;
                MoveNext = e.MoveNext;
                Reset = e.Reset;
                _getValue = e.GetValue;
                _getKey = e.GetKey;
            }
        }

        /// <summary>
        /// 异常文本
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// 键
        /// </summary>
        public object Key => _getKey();

        /// <summary>
        /// 值
        /// </summary>
        public object Value => _getValue();

        public DictionaryEntry Entry =>
            _enumerator?.Entry ?? new DictionaryEntry(_getKey(), _getValue());

        object IEnumerator.Current => Entry;

        bool IEnumerator.MoveNext() => MoveNext();

        void IEnumerator.Reset() => Reset();
    }
}