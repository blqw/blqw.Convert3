using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CDataTable : AdvancedConvertor<DataTable>
    {

        static IConvertor<string> _keyConvertor;

        protected override void Initialize()
        {
            _keyConvertor = Convert3.GetConvertor<string>();
        }

        private static IEnumerator GetIEnumerator(object input)
        {
            var emtr = input as IEnumerator;
            if (emtr != null)
            {
                return emtr;
            }
            var emab = input as IEnumerable;
            if (emab != null)
            {
                return emab.GetEnumerator();
            }
            var ls = input as IListSource;
            if (ls != null)
            {
                return ls.GetList().GetEnumerator();
            }
            var row = input as DataRow;
            if (row != null)
            {
                return row.ItemArray.GetEnumerator(); ;
            }
            var rv = input as DataRowView;
            if (rv != null)
            {
                return rv.Row.ItemArray.GetEnumerator(); ;
            }
            return null;
        }

        protected override bool Try(object input, Type outputType, out DataTable result)
        {
            var view = input as DataView;
            if (view != null)
            {
                result = view.ToTable();
                return true;
            }
            var reader = input as IDataReader;
            if (reader != null)
            {
                if (reader.IsClosed)
                {
                    ErrorContext.Error = new NotImplementedException("DataReader已经关闭");
                    result = null;
                    return false;
                }
                var table = new DataTable();
                table.Load(reader);
                result = table;
                return true;
            }

            var emtr = GetIEnumerator(input);
            if (emtr == null)
            {
                ErrorContext.Error = new InvalidCastException("目前仅支持DataView,DataRow,DataRowView,或实现IEnumerator,IEnumerable,IListSource,IDataReader接口的对象转向DataTable");
                result = null;
                return false;
            }
            var type = input.GetType();
            var genericTypes = type.GetGenericArguments();
            if (genericTypes.Length != 1)
            {
                ErrorContext.Error = new NotImplementedException("当泛型参数超过2个时,无法确定内部元素的类型");
                result = null;
                return false;
            }

            var helper = GetHelper(genericTypes[0]);
            while (emtr.MoveNext())
            {
                if (helper.AddRow(emtr.Current) == false)
                {
                    result = null;
                    return false;
                }
            }
            result = helper.GetTable();
            return true;
        }

        private IConvertHelper GetHelper(Type elementType)
        {
            if (typeof(NameValueCollection).IsAssignableFrom(elementType))
            {
                return new ConvertHelper1(1);
            }
            bool hasIDictionary = false;
            foreach (var iface in elementType.GetInterfaces())
            {
                if (iface.IsGenericType)
                {
                    if (iface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        return (IConvertHelper)Activator.CreateInstance(typeof(ConvertHelper2<,>).MakeGenericType(iface.GetGenericArguments()), 1);
                    }
                }
                if (iface == typeof(IDictionary))
                {
                    hasIDictionary = true;
                }
            }
            if (hasIDictionary)
            {
                return new ConvertHelper3(1);
            }
            return new ConvertHelper4(elementType);
        }

        interface IConvertHelper
        {
            bool AddRow(object value);
            DataTable GetTable();
        }
        #region inner
        struct ConvertHelper1 : IConvertHelper //NameValueCollection
        {
            DataTable _table;
            DataColumnCollection _columns;
            public ConvertHelper1(int i)
            {
                _table = new DataTable();
                _columns = _table.Columns;
            }

            public DataTable GetTable()
            {
                return _table;
            }

            public bool AddRow(object element)
            {
                var nv = (NameValueCollection)element;
                var row = _table.NewRow();
                foreach (string name in nv)
                {
                    if (_columns.Contains(name))
                    {
                        row[name] = nv[name];
                    }
                    else
                    {
                        var col = _columns.Add(name, typeof(string));
                        row[col] = nv[name];
                    }
                }
                _table.Rows.Add(row);
                return true;
            }
        }
        struct ConvertHelper2<K, V> : IConvertHelper //IDictionaryT
        {
            DataTable _table;
            DataColumnCollection _columns;
            static IConvertor<V> _valueConvertor = Convert3.GetConvertor<V>();
            public ConvertHelper2(int i)
            {
                _table = new DataTable();
                _columns = _table.Columns;
            }

            public DataTable GetTable()
            {
                return _table;
            }

            public bool AddRow(object element)
            {
                var dict = (IDictionary<K, V>)element;
                var row = _table.NewRow();
                foreach (var item in dict)
                {
                    string name;
                    if ((name = item.Key as string) == null
                        && _keyConvertor.Try(item.Key, null, out name) == false)
                    {
                        ErrorContext.Error = new NotImplementedException("很奇怪的问题");
                        return false;
                    }
                    else if (name.Length > 50)
                    {
                        ErrorContext.Error = new NotImplementedException(name + " 作为DataTable的列名太长");
                        return false;
                    }
                    if (_columns.Contains(name))
                    {
                        row[name] = item.Value;
                    }
                    else
                    {
                        var col = _columns.Add(name, typeof(V));
                        row[col] = item.Value;
                    }
                }
                _table.Rows.Add(row);
                return true;
            }
        }
        struct ConvertHelper3 : IConvertHelper //IDictionary
        {
            DataTable _table;
            DataColumnCollection _columns;
            public ConvertHelper3(int i)
            {
                _table = new DataTable();
                _columns = _table.Columns;
                _keyConvertor = Convert3.GetConvertor<string>();
            }

            public DataTable GetTable()
            {
                return _table;
            }

            public bool AddRow(object element)
            {
                var dict = (IDictionary)element;
                var row = _table.NewRow();
                foreach (DictionaryEntry item in dict)
                {
                    string name;
                    if ((name = item.Key as string) == null
                        && _keyConvertor.Try(item.Key, null, out name) == false)
                    {
                        return false;
                    }
                    else if (name.Length > 50)
                    {
                        ErrorContext.Error = new NotImplementedException(name + " 作为DataTable的列名太长");
                        return false;
                    }
                    if (_columns.Contains(name))
                    {
                        row[name] = item.Value;
                    }
                    else
                    {
                        var col = _columns.Add(name, typeof(object));
                        row[col] = item.Value;
                    }
                }
                _table.Rows.Add(row);
                return true;
            }
        }
        struct ConvertHelper4 : IConvertHelper //Entity
        {
            DataTable _table;
            PropertyHandler[] _properties;
            public ConvertHelper4(Type type)
            {
                _table = new DataTable();
                _properties = PublicPropertyCache.GetByType(type);
                foreach (var p in _properties)
                {
                    _table.Columns.Add(p.Name, p.Property.PropertyType);
                }
            }

            public DataTable GetTable()
            {
                return _table;
            }

            public bool AddRow(object element)
            {
                var dict = element;
                var row = _table.NewRow();
                foreach (var p in _properties)
                {
                    if (p.Get != null)
                    {
                        row[p.Name] = p.Get(element);
                    }
                }
                _table.Rows.Add(row);
                return true;
            }
        }
        #endregion

        protected override bool Try(string input, Type outputType, out DataTable result)
        {
            input = input.Trim();
            if (input[0] == '{' && input[input.Length - 1] == '}')
            {
                return CJsonObject.TryTo(input, outputType, out result);
            }
            result = null;
            return false;
        }
    }
}
