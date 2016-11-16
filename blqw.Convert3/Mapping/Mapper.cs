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
        private DataReaderEnumerator _reader;
        private NameValueEnumerator _nv;
        private DataRowEnumerator _row;
        private DataSetEnumerator _dataSet;
        private readonly IDictionaryEnumerator _enumerator;
        private PairEnumerator _pair;
        private PropertyEnumerator _property;
        private readonly int _index;
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
                _reader = new DataReaderEnumerator(reader);
                Error = _reader.Error;
                _index = 1;
                return;
            }

            var no = input as NameObjectCollectionBase;
            if (no != null)
            {
                _nv = new NameValueEnumerator(no);
                Error = _nv.Error;
                _index = 2;
                return;
            }

            var row = (input as DataRowView)?.Row ?? input as DataRow;
            if (row?.Table != null)
            {
                _row = new DataRowEnumerator(row);
                Error = _row.Error;
                _index = 3;
                return;
            }

            var dataset = input as DataSet;
            if (dataset != null)
            {
                _dataSet = new DataSetEnumerator(dataset);
                Error = _dataSet.Error;
                _index = 4;
                return;
            }

            var dict = input as IDictionary;
            if (dict != null)
            {
                _enumerator = dict.GetEnumerator();
                _index = 5;
                return;
            }

            var ee = (input as IEnumerable)?.GetEnumerator() ?? input as IEnumerator;
            if (ee != null)
            {
                _pair = new PairEnumerator(ee);
                Error = _pair.Error;
                _index = 6;
                return;
            }

            var ps = PublicPropertyCache.GetByType(input.GetType());
            if (ps.Length > 0)
            {
                _property = new PropertyEnumerator(input, ps);
                Error = _property.Error;
                _index = 7;
            }
        }

        /// <summary>
        /// 异常文本
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// 键
        /// </summary>
        public object Key
        {
            get
            {
                switch (_index)
                {
                    case 1:
                        return _reader.GetKey();
                    case 2:
                        return _nv.GetKey();
                    case 3:
                        return _row.GetKey();
                    case 4:
                        return _dataSet.GetKey();
                    case 5:
                        return _enumerator.Key;
                    case 6:
                        return _pair.GetKey();
                    case 7:
                        return _property.GetKey();
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get
            {
                switch (_index)
                {
                    case 1:
                        return _reader.GetValue();
                    case 2:
                        return _nv.GetValue();
                    case 3:
                        return _row.GetValue();
                    case 4:
                        return _dataSet.GetValue();
                    case 5:
                        return _enumerator.Value;
                    case 6:
                        return _pair.GetValue();
                    case 7:
                        return _property.GetValue();
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public DictionaryEntry Entry =>
            _enumerator?.Entry ?? new DictionaryEntry(Key, Value);

        object IEnumerator.Current => Entry;

        public bool MoveNext()
        {
            switch (_index)
            {
                case 1:
                    return _reader.MoveNext();
                case 2:
                    return _nv.MoveNext();
                case 3:
                    return _row.MoveNext();
                case 4:
                    return _dataSet.MoveNext();
                case 5:
                    return _enumerator.MoveNext();
                case 6:
                    return _pair.MoveNext();
                case 7:
                    return _property.MoveNext();
                default:
                    throw new NotSupportedException();
            }
        }

        public void Reset()
        {
            switch (_index)
            {
                case 1:
                     _reader.Reset();
                    break;
                case 2:
                     _nv.Reset();
                    break;
                case 3:
                     _row.Reset();
                    break;
                case 4:
                     _dataSet.Reset();
                    break;
                case 5:
                     _enumerator.Reset();
                    break;
                case 6:
                     _pair.Reset();
                    break;
                case 7:
                     _property.Reset();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}