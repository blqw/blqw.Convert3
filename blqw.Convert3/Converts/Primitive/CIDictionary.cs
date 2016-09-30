using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using blqw.IOC;

namespace blqw.Converts
{
    internal sealed class CIDictionary : BaseTypeConvertor<IDictionary>
    {
        protected override IDictionary ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            success = true;
            if ((input == null) || input is DBNull)
            {
                return null;
            }

            var helper = new DictionaryHelper(outputType, context);
            if (helper.CreateInstance() == false)
            {
                success = false;
                return null;
            }

            var reader = input as IDataReader;
            if (reader != null)
            {
                if (reader.IsClosed)
                {
                    context.AddException("DataReader已经关闭");
                    success = false;
                    return null;
                }
                for (var i = 0; i < reader.FieldCount; i++)
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


            var row = (input as DataRowView)?.Row ?? input as DataRow;
            if (row?.Table != null)
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

            var ee = (input as IDictionary)?.GetEnumerator();
            if (ee != null)
            {
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
                    if ((p.Get != null) && (helper.Add(p.Name, p.Get(input)) == false))
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

        protected override IDictionary ChangeType(ConvertContext context, string input, Type outputType,
            out bool success)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                success = false;
                return null;
            }
            input = input.Trim();
            if ((input[0] == '{') && (input[input.Length - 1] == '}'))
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (IDictionary)result;
                }
                catch (Exception ex)
                {
                    context.AddException(ex);
                }
            }
            success = false;
            return null;
        }


        private struct DictionaryHelper
        {
            public IDictionary Dictionary;
            private readonly Type _type;
            private readonly ConvertContext _context;

            public DictionaryHelper(Type type, ConvertContext context)
            {
                _type = type;
                _context = context;
                Dictionary = null;
            }

            public bool Add(object key, object value)
            {
                try
                {
                    Dictionary.Add(key, value);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException($"向字典{CType.GetFriendlyName(_type)}中添加元素 {key} 失败,原因:{ex.Message}", ex);
                    return false;
                }
            }

            internal bool CreateInstance()
            {
                if (_type.IsInterface)
                {
                    Dictionary = new Hashtable();
                    return true;
                }
                try
                {
                    Dictionary = (IDictionary)Activator.CreateInstance(_type);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException(ex);
                    return false;
                }
            }
        }
    }
}