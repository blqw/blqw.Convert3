using blqw.IOC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CNameValueCollection : AdvancedConvertor<NameValueCollection>
    {
        protected override NameValueCollection ChangeType(object input, Type outputType, out bool success)
        {
            if (input == null || input is DBNull)
            {
                success = true;
                return null;
            }
            var helper = new NVCollectiontHelper(outputType);
            if (helper.CreateInstance() == false)
            {
                success = false;
                return null;
            }
            success = true;
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
                return helper.Collection;
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
                return helper.Collection;
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
                var cols = Enumerable.Range(0, reader.FieldCount).Select(i => new { name = reader.GetName(i), i });
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (helper.Add(reader.GetName(i), reader.GetValue, i) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Collection;
            }

            var dict = input as IDictionary;
            if (dict != null)
            {
                foreach (DictionaryEntry item in dict)
                {
                    if (helper.Add(item.Key, item.Value) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Collection;
            }

            var ps = PublicPropertyCache.GetByType(input.GetType());
            if (ps.Length > 0)
            {
                foreach (var p in ps)
                {
                    if (p.Get != null && helper.Add(p.Name, p.Get, input) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return helper.Collection;
            }
            success = false;
            return null;
        }

        protected override NameValueCollection ChangeType(string input, Type outputType, out bool success)
        {
            if (input[0] == '{' && input[input.Length - 1] == '}')
            {
                try
                {
                    var result = Components.ToJsonObject(outputType, input);
                    success = true;
                    return (NameValueCollection)result;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                }
            }
            success = false;
            return null;
        }


        struct NVCollectiontHelper
        {
            public NameValueCollection Collection;
            private Type _type;
            public NVCollectiontHelper(Type type)
            {
                _type = type;
                Collection = null;
            }

            internal bool CreateInstance()
            {
                try
                {
                    Collection = (NameValueCollection)Activator.CreateInstance(_type);
                    return true;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                    return false;
                }
            }
            public bool Add(object key, object value)
            {
                var conv = ConvertorContainer.StringConvertor;
                bool b;
                var skey = conv.ChangeType(key, typeof(string), out b);
                if (b == false)
                {
                    return false;
                }
                var svalue = conv.ChangeType(value, typeof(string), out b);
                if (b == false)
                {
                    return false;
                }
                try
                {
                    Collection.Add(skey, svalue);
                    return true;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                    return false;
                }
            }

            public bool Add<P>(object key, Func<P, object> getValue, P param)
            {
                var conv = ConvertorContainer.StringConvertor;
                bool b;
                var skey = conv.ChangeType(key, typeof(string), out b);
                if (b == false)
                {
                    return false;
                }
                try
                {
                    var value = getValue(param);
                    var svalue = conv.ChangeType(value, typeof(string), out b);
                    if (b == false)
                    {
                        return false;
                    }
                    Collection.Add(skey, svalue);
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
