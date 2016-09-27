using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using blqw.IOC;

namespace blqw.Converts
{
    public class CIDictionary<K, V> : BaseTypeConvertor<IDictionary<K, V>>
    {
        protected override IDictionary<K, V> ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            if (input == null || input is DBNull)
            {
                return null;
            }

            var keyConvertor = context.Get<K>();
            var valueConvertor = context.Get<V>();


            var helper = new DictionaryHelper(context, outputType, keyConvertor, valueConvertor);
            if (helper.CreateInstance() == false)
            {
                success = false;
                return null;
            }

            var reader = input as IDataReader;
            if (reader != null)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (helper.Add(reader.GetName(i), reader.GetValue(i)) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Dictionary;
            }


            var nv = input as NameValueCollection;
            if (nv != null)
            {
                foreach (string name in nv)
                {
                    if (helper.Add(name, nv[name]) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Dictionary;
            }


            var row = (input as DataRowView)?.Row ?? (input as DataRow);
            if (row != null && row.Table != null)
            {
                var cols = row.Table.Columns;
                foreach (DataColumn col in cols)
                {
                    if (helper.Add(col.ColumnName, row[col]) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Dictionary;
            }

            var dataset = input as DataSet;
            if (dataset != null)
            {
                for (int i = 0, length = dataset.Tables.Count; i < length; i++)
                {
                    var table = dataset.Tables[i];
                    if (helper.Add(table.TableName ?? $"table_{i}", table) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Dictionary;
            }

            var dict = input as IDictionary;
            if (dict != null)
            {
                var ee = dict.GetEnumerator();
                while (ee.MoveNext())
                {
                    if (helper.Add(ee.Key, ee.Value) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Dictionary;
            }

            var ps = PublicPropertyCache.GetByType(input.GetType());
            if (ps.Length > 0)
            {
                foreach (var p in ps)
                {
                    if (p.Get != null && helper.Add(p.Name, p.Get(input)) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Dictionary;
            }

            success = false;
            return null;
        }

        protected override IDictionary<K, V> ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            input = input?.Trim();
            if (input?.Length > 2)
            {
                if (input[0] == '{' && input[input.Length - 1] == '}')
                {
                    try
                    {
                        var result = ComponentServices.ToJsonObject(outputType, input);
                        success = true;
                        return (IDictionary<K, V>)result;
                    }
                    catch (Exception ex)
                    {
                        Error.Add(ex);
                    }
                }
            }
            success = false;
            return null;
        }
        

        struct DictionaryHelper
        {
            public IDictionary<K, V> Dictionary;
            private readonly IConvertor<K> _keyConvertor;
            private readonly ConvertContext _context;
            private readonly Type _type;
            private readonly IConvertor<V> _valueConvertor;

            public DictionaryHelper(ConvertContext context, Type type, IConvertor<K> keyConvertor, IConvertor<V> valueConvertor)
            {
                _context = context;
                _type = type;
                Dictionary = null;
                _keyConvertor = keyConvertor;
                _valueConvertor = valueConvertor;
            }

            public bool Add(object key, object value)
            {
                bool b;
                var k = _keyConvertor.ChangeType(_context,key, _keyConvertor.OutputType, out b);
                if (b == false)
                {
                    Error.Add(new NotSupportedException($"添加到字典{CType.GetFriendlyName(_type)}失败"));
                    return false;
                }

                var v = _valueConvertor.ChangeType(_context, value, _valueConvertor.OutputType, out b);
                if (b == false)
                {
                    Error.Add(new NotSupportedException($"向字典{CType.GetFriendlyName(_type)}中添加元素 {key} 失败"));
                    return false;
                }
                try
                {
                    Dictionary.Add(k, v);
                    return true;
                }
                catch (Exception ex)
                {
                    Error.Add(new NotSupportedException($"向字典{CType.GetFriendlyName(_type)}中添加元素 {key} 失败,原因:{ex.Message}", ex));
                    return false;
                }
            }

            internal bool CreateInstance()
            {
                if (_type.IsInterface)
                {
                    Dictionary = new Dictionary<K, V>();
                    return true;
                }
                try
                {
                    Dictionary = (IDictionary<K, V>)Activator.CreateInstance(_type);
                    return true;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                    return false;
                }
            }
        }
    }
}