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
    class CNameValueCollection : AdvancedConvertor<NameValueCollection>
    {
        static IConvertor<string> _stringConvertor;

        protected override void Initialize()
        {
            _stringConvertor = Convert3.GetConvertor<string>();
        }

        protected override bool Try(object input, Type outputType, out NameValueCollection result)
        {
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
                result = arg.Collection;
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
                result = arg.Collection;
                return true;
            }

            var reader = input as IDataReader;
            if (reader != null)
            {
                if (reader.IsClosed)
                {
                    ErrorContext.Error = new InvalidCastException("DataReader已经关闭");
                    result = null;
                    return false;
                }
                var arg = new ConvertHelper(outputType);
                var cols = Enumerable.Range(0, reader.FieldCount).Select(i => new { name = reader.GetName(i), i });
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (arg.Add(reader.GetName(i), reader.GetValue, i) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = arg.Collection;
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
                result = arg.Collection;
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
                result = arg.Collection;
                return true;
            }

            ErrorContext.CastFail(input, outputType);
            result = null;
            return false;
        }


        struct ConvertHelper
        {
            public readonly NameValueCollection Collection;
            public ConvertHelper(Type type)
            {
                Collection = (NameValueCollection)Activator.CreateInstance(type);
            }

            public bool Add(object key, object value)
            {
                string skey, svalue;
                if (_stringConvertor.Try(key, null, out skey) == false)
                {
                    return false;
                }
                if (_stringConvertor.Try(value, null, out svalue) == false)
                {
                    return false;
                }
                Collection.Add(skey, svalue);
                return true;
            }

            public bool Add<P>(object key, Func<P, object> getValue, P param)
            {
                string skey, svalue;
                if (_stringConvertor.Try(key, null, out skey) == false)
                {
                    return false;
                }
                var value = getValue(param);
                if (_stringConvertor.Try(value, null, out svalue) == false)
                {
                    return false;
                }
                Collection.Add(skey, svalue);
                return true;
            }
        }


        protected override bool Try(string input, Type outputType, out NameValueCollection result)
        {
            if (input[0] == '{' && input[input.Length - 1] == '}')
            {
                return CJsonObject.TryTo(input, outputType, out result);
            }
            result = null;
            return false;
        }
    }
}
