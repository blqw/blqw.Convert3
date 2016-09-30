using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Dynamic
{
    public class DynamicList : DynamicObject, IList, IObjectHandle, IObjectReference
    {
        IList _list;

        public DynamicList()
        {
            _list = new ArrayList();
        }


        public DynamicList(IList list)
        {
            _list = list ?? new ArrayList();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return new string[] { "Count", "Length" };
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (typeof(IConvertible).IsAssignableFrom(binder.ReturnType))
            {
                if (_list.Count == 1 && Convert3.TryChangedType(_list[0], binder.ReturnType, out result))
                {
                    return true;
                }
            }
            return Convert3.TryChangedType(_list, binder.ReturnType, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name.ToLower();
            if ("count,length,".IndexOf(binder.Name + ",", StringComparison.OrdinalIgnoreCase) > -1)
            {
                result = _list.Count;
                return true;
            }
            result = null;
            return false;
        }

        private int Indexer(object[] indexes)
        {
            if (indexes == null || indexes.Length != 1)
            {
                return -1;
            }
            var i = indexes[0].To<int>(-1);
            if (i < 0 || i >= _list.Count)
            {
                return -1;
            }
            return i;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var i = Indexer(indexes);
            if (i >= 0 && Convert3.TryChangedType(_list[i], binder.ReturnType, out result))
            {
                result = Convert3.ToDynamic(result);
                return true;
            }

            result = DynamicPrimitive.Null;
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (IsReadOnly)
            {
                return false;
            }

            var i = Indexer(indexes);
            if (i < 0)
            {
                return false;
            }
            _list[i] = value;
            return true;
        }

        public bool IsReadOnly { get; set; }


        int IList.Add(object value)
        {
            _list.Add(value);
            return _list.Count;
        }

        void IList.Clear()
        {
            _list.Clear();
        }

        bool IList.Contains(object value)
        {
            return _list.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return _list.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            _list.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            _list.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                if (index >= 0 && index < _list.Count)
                {
                    return _list[index];
                }
                return null;
            }
            set
            {
                _list[index] = value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            _list.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return _list.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return _list; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in _list)
            {
                if (item == null)
                {
                    yield return DynamicPrimitive.Null;
                }
                yield return Convert3.ToDynamic(item);
            }
        }


        /// <summary>打开该对象。</summary>
        /// <returns>已打开的对象。</returns>
        public object Unwrap() => _list;

        /// <summary>返回应进行反序列化的真实对象（而不是序列化流指定的对象）。</summary>
        /// <returns>返回放入图形中的实际对象。</returns>
        /// <param name="context">当前对象从其中进行反序列化的 <see cref="T:System.Runtime.Serialization.StreamingContext" />。</param>
        /// <exception cref="T:System.Security.SecurityException">调用方没有所要求的权限。无法对中等信任的服务器进行调用。</exception>
        public object GetRealObject(StreamingContext context) => _list;
    }
}
