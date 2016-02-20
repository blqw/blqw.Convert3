using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Dynamic
{
    public class DynamicDictionary : DynamicObject, IDictionary, IFormatProvider
    {

        object IFormatProvider.GetFormat(Type formatType)
        {
            if (formatType != null && string.Equals("Json", formatType.Name, StringComparison.Ordinal))
            {
                return _dict;
            }
            return null;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var key in _dict.Keys)
            {
                var str = key as string;
                if (str != null)
                {
                    yield return str;
                }
            }
        }



        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (typeof(IConvertible).IsAssignableFrom(binder.ReturnType) && _dict.Count == 1)
            {
                var ee = _dict.Values.GetEnumerator();
                ee.MoveNext();
                if (Convert3.TryChangedType(ee.Current, binder.ReturnType, out result))
                {
                    return true;
                }
            }
            return Convert3.TryChangedType(_dict, binder.ReturnType, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _dict[binder.Name];
            if (result != null)
            {
                if (Convert3.TryChangedType(result, binder.ReturnType, out result))
                {
                    result = Convert3.ToDynamic(result);
                    return true;
                }
            }
            result = DynamicSystemObject.Null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (IsReadOnly)
            {
                return false;
                //throw new NotSupportedException("当前对象是只读的");
            }
            _dict[binder.Name] = value;
            return true;
        }

        private string GetIndexer0(object[] indexes)
        {
            if (indexes == null || indexes.Length != 1)
            {
                return null;
            }
            return indexes[0] as string;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var key = GetIndexer0(indexes);
            if (key == null)
            {
                result = DynamicSystemObject.Null;
                return true;
            }
            result = _dict[key];
            if (result != null)
            {
                if (Convert3.TryChangedType(result, binder.ReturnType, out result))
                {
                    result = result as DynamicObject ?? Convert3.ToDynamic(result);
                    return true;
                }
            }
            result = DynamicSystemObject.Null;
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (IsReadOnly)
            {
                return false;
                //throw new NotSupportedException("当前对象是只读的");
            }
            var key = GetIndexer0(indexes);
            if (key == null)
            {
                return false;
            }
            _dict[key] = value;
            return true;
        }

        public bool IsReadOnly { get; set; }

        #region 必要属性构造函数
        IDictionary _dict;

        public DynamicDictionary()
        {
            _dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public DynamicDictionary(StringComparer comparer)
        {
            _dict = new Dictionary<string, object>(comparer);
        }

        public DynamicDictionary(IDictionary dict)
        {
            _dict = dict;
        }

        #endregion

        #region 显示实现接口




        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }



        void IDictionary.Add(object key, object value)
        {
            _dict.Add(key, value);
        }

        void IDictionary.Clear()
        {
            _dict.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return _dict.Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        struct MyEnumerator : IDictionaryEnumerator
        {
            IDictionaryEnumerator _enumerator;
            public MyEnumerator(IDictionaryEnumerator enumerator)
            {
                _enumerator = enumerator;
            }
            public object Current
            {
                get
                {
                    return Dynamic(_enumerator.Current);
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    var entry = _enumerator.Entry;
                    return new DictionaryEntry(Dynamic(entry.Key),Dynamic(entry.Value));
                }
            }

            private dynamic Dynamic(object obj)
            {
                if (obj == null)
                {
                    return DynamicSystemObject.Null;
                }
                return Convert3.ToDynamic(obj);
            }

            public object Key
            {
                get
                {
                    return Dynamic(_enumerator.Key);
                }
            }

            public object Value
            {
                get
                {
                    return Dynamic(_enumerator.Value);
                }
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get { return _dict.Keys; }
        }

        void IDictionary.Remove(object key)
        {
            _dict.Remove(key);
        }

        ICollection IDictionary.Values
        {
            get { return _dict.Values; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                _dict[key] = value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_dict).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return _dict.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return _dict; }
        }
        #endregion

    }
}
