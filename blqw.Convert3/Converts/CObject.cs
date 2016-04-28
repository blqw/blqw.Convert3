using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CObject : AdvancedConvertor<object>
    {
        protected override object ChangeType(object input, Type outputType, out bool success)
        {
            if (input == null || input is DBNull)
            {
                success = true;
                return null;
            }
            var obj = new SetObjectProperty(outputType);
            if (obj.CreateInstance() == false)
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
                    if (obj.Set(name, nv[name]) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return obj.Instance;
            }

            var row = (input as DataRowView)?.Row ?? (input as DataRow);
            if (row != null && row.Table != null)
            {
                var cols = row.Table.Columns;
                foreach (DataColumn col in cols)
                {
                    if (obj.Set(col.ColumnName, row[col]) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return obj.Instance;
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
                    if (obj.Set(reader.GetName(i), reader.GetValue, i) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return obj.Instance;
            }

            var dict = input as IDictionary;
            if (dict != null)
            {
                foreach (DictionaryEntry item in dict)
                {
                    var name = item.Key as string;
                    if (name != null && obj.Set(name, item.Value) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return obj.Instance;
            }

            var ps = PublicPropertyCache.GetByType(input.GetType());
            if (ps.Length > 0)
            {
                foreach (var p in ps)
                {
                    if (p.Get != null && obj.Set(p.Name, p.Get, input) == false)
                    {
                        success = false;
                        return null;
                    }
                }
                return obj.Instance;
            }
            if (outputType == typeof(object))
            {
                success = true;
                return input;
            }
            success = false;
            return null;

        }

        protected override object ChangeType(string input, Type outputType, out bool success)
        {
            try
            {
                if (outputType == typeof(object))
                {
                    success = true;
                    return input;
                }
                var type = Type.GetType(input, false, true);
                if (type == null)
                {
                    success = false;
                    return null;
                }
                else if (outputType.IsAssignableFrom(type) == false)
                {
                    success = false;
                    return null;
                }

                outputType = type;
                success = true;
                return Activator.CreateInstance(type, true);
            }
            catch (Exception ex)
            {
                Error.Add(ex);
            }
            success = false;
            return null;
        }


        struct SetObjectProperty
        {
            PropertyHandler[] _properties;
            int _propertyCount;
            public object Instance;
            public IDictionary<string, object> Dynamic;
            Type _type;
            public SetObjectProperty(Type type)
            {
                _properties = PublicPropertyCache.GetByType(type);
                _propertyCount = _properties.Length;
                if (type == typeof(object))
                {
                    _type = null;
                    Dynamic = new System.Dynamic.ExpandoObject();
                }
                else
                {
                    _type = type;
                    Dynamic = null;
                }
                Instance = Dynamic;
            }

            public bool CreateInstance()
            {
                if (Dynamic != null)
                {
                    return true;
                }
                try
                {
                    Instance = Activator.CreateInstance(_type);
                    return true;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                    return false;
                }
            }

            private PropertyHandler GetProperty(string name)
            {
                for (int i = 0; i < _propertyCount; i++)
                {
                    var p = _properties[i];
                    if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return p;
                    }
                }
                return null;
            }

            public bool Set(string name, object value)
            {
                if (Dynamic != null)
                {
                    Dynamic[name] = value;
                    return true;
                }
                var p = GetProperty(name);
                if (p?.Set != null)
                {
                    return p.SetValue(Instance, value);
                }
                return true;
            }

            public bool Set<P>(string name, Func<P, object> getValue, P param)
            {
                if (Dynamic != null)
                {
                    Dynamic[name] = getValue(param);
                    return true;
                }
                var p = GetProperty(name);
                if (p?.Set != null)
                {
                    var value = getValue(param);
                    return p.SetValue(Instance, value);
                }
                return true;
            }
        }
    }
}
