using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Dynamic
{
    public class DynamicNameValueCollection : DynamicObject,IFormatProvider
    {
        object IFormatProvider.GetFormat(Type formatType)
        {
            if (formatType != null && string.Equals("Json", formatType.Name, StringComparison.Ordinal))
            {
                return _items;
            }
            return null;
        }

        NameValueCollection _items;
        public DynamicNameValueCollection()
        {
            _items = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
        }

        public DynamicNameValueCollection(StringComparer comparer)
        {
            _items = new NameValueCollection(comparer);
        }

        public DynamicNameValueCollection(NameValueCollection nv)
        {
            _items = nv;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _items.AllKeys;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (typeof(IConvertible).IsAssignableFrom(binder.ReturnType))
            {
                if (_items.Count == 1 && Convert3.TryChangedType(_items[0], binder.ReturnType, out result))
                {
                    return true;
                }
            }
            return Convert3.TryChangedType(_items, binder.ReturnType, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _items[binder.Name];
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
            _items[binder.Name] = value.To<string>();
            return true;
        }

        private object Indexer(object[] indexes)
        {
            if (indexes == null || indexes.Length != 1)
            {
                return null;
            }
            var index = indexes[0];
            var name = index as string;
            if (name != null)
            {
                return _items[name];
            }
            var i = index.To<int>(-1);
            if (i < 0 || i >= _items.Count)
            {
                return null;
            }
            return _items[i];
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Indexer(indexes);
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

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (IsReadOnly)
            {
                return false;
            }

            if (indexes == null || indexes.Length != 1)
            {
                return false;
            }
            var index = indexes[0];
            var name = index as string;
            var str = value.To<string>();
            if (name != null)
            {
                _items[name] = str;
                return true;
            }
            var i = index.To<int>(-1);
            if (i < 0 || i >= _items.Count)
            {
                return false;
            }
            _items[_items.GetKey(i)] = str;
            return true;
        }

        public bool IsReadOnly { get; set; }

    }
}
