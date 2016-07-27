using blqw.IOC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CIDictionary : AdvancedConvertor<IDictionary>
    {
        protected override IDictionary ChangeType(object input, Type outputType, out bool success)
        {
            success = true;
            if (input == null || input is DBNull)
            {
                return null;
            }

            var helper = new DictionaryHelper(outputType);
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
                    Error.Add(new NotImplementedException("DataReader已经关闭"));
                    success = false;
                    return null;
                }
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

        protected override IDictionary ChangeType(string input, Type outputType, out bool success)
        {
            input = input.Trim();
            if (input[0] == '{' && input[input.Length - 1] == '}')
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (IDictionary)result;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                }
            }
            success = false;
            return null;
        }



        struct DictionaryHelper
        {
            public IDictionary Dictionary;
            private Type _type;
            public DictionaryHelper(Type type)
            {
                _type = type;
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
                    Error.Add(new NotSupportedException($"向字典{CType.GetFriendlyName(_type)}中添加元素 {key} 失败,原因:{ex.Message}", ex));
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
                    Error.Add(ex);
                    return false;
                }
            }
        }

    }
}
