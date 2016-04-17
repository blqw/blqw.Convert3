using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CIDictionary<K, V> : AdvancedConvertor<IDictionary<K, V>>
    {
        static Type _keyType = typeof(K);
        static Type _valueType = typeof(V);

        static IConvertor<K> _keyConvertor;
        static IConvertor<V> _valueConvertor;
        protected override void Initialize()
        {
            if (_keyConvertor == null && _valueConvertor == null)
            {
                _keyConvertor = Convert3.GetConvertor<K>();
                _valueConvertor = Convert3.GetConvertor<V>();
            }
        }
        protected override bool Try(object input, Type outputType, out IDictionary<K, V> result)
        {
            var reader = input as IDataReader;
            if (reader != null)
            {
                if (reader.IsClosed)
                {
                    ErrorContext.Error = new NotImplementedException("DataReader已经关闭");
                    result = null;
                    return false;
                }
                var arg = new ConvertHelper(outputType);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (arg.Add(reader.GetName(i), reader.GetValue, i) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = arg.Dictionary;
                return true;
            }

            var nv = input as NameValueCollection;
            if (nv != null)
            {
                var arg = new ConvertHelper(outputType);
                foreach (string name in nv)
                {
                    if (arg.Add(name, nv[name]) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = arg.Dictionary;
                return true;
            }

            var rv = input as DataRowView;
            DataRow row;
            if (rv != null)
            {
                row = rv.Row;
            }
            else
            {
                row = input as DataRow;
            }
            if (row != null && row.Table != null)
            {
                var arg = new ConvertHelper(outputType);
                var cols = row.Table.Columns;
                foreach (DataColumn col in cols)
                {
                    if (arg.Add(col.ColumnName, row[col]) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = arg.Dictionary;
                return true;
            }

            var dict = input as IDictionary;
            if (dict != null)
            {
                var arg = new ConvertHelper(outputType);
                foreach (DictionaryEntry item in dict)
                {
                    if (arg.Add(item.Key, item.Value) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = arg.Dictionary;
                return true;
            }


            var ps = PublicPropertyCache.GetByType(input.GetType());
            if (ps.Length > 0)
            {
                var arg = new ConvertHelper(outputType);
                foreach (var p in ps)
                {
                    if (p.Get != null && arg.Add(p.Name, p.Get, input) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = arg.Dictionary;
                return true;
            }
            ErrorContext.CastFail(input, outputType);
            result = null;
            return false;
        }

        struct ConvertHelper
        {
            public readonly IDictionary<K, V> Dictionary;

            public ConvertHelper(Type type)
            {
                if (type.IsInterface)
                {
                    Dictionary = new Dictionary<K, V>();
                }
                else
                {
                    Dictionary = (IDictionary<K, V>)Activator.CreateInstance(type);
                }
            }

            public bool Add(object key, object value)
            {
                if (_keyConvertor == null || _valueConvertor == null)
                {
                    ErrorContext.ConvertorNotFound(
                        _keyConvertor == null ? _keyType : _valueType);
                    return false;
                }
                K k;
                if (_keyConvertor.Try(key, _keyType, out k) == false)
                {
                    ErrorContext.Error = new NotImplementedException("Dictionary键转换失败");
                    return false;
                }
                V v;
                if (_valueConvertor.Try(value, _valueType, out v) == false)
                {
                    ErrorContext.Error = new NotImplementedException("Dictionary值转换失败");
                    return false;
                }
                Dictionary.Add(k, v);
                return true;
            }
            public bool Add<P>(object key, Func<P, object> getValue, P param)
            {
                if (_keyConvertor == null || _valueConvertor == null)
                {
                    ErrorContext.ConvertorNotFound(
                        _keyConvertor == null ? _keyType : _valueType);
                    return false;
                }

                K k;
                if (_keyConvertor.Try(key, _keyType, out k) == false)
                {
                    ErrorContext.Error = new NotImplementedException("Dictionary键转换失败");
                    return false;
                }
                V v;
                if (_valueConvertor.Try(getValue(param), _valueType, out v) == false)
                {
                    ErrorContext.Error = new NotImplementedException("Dictionary值转换失败");
                    return false;
                }
                Dictionary.Add(k, v);
                return true;
            }
        }

        protected override bool Try(string input, Type outputType, out IDictionary<K, V> result)
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
