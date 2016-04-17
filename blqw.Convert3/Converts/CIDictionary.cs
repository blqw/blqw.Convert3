using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CIDictionary : AdvancedConvertor<IDictionary>
    {
        protected override bool Try(object input, Type outputType, out IDictionary result)
        {
            if (outputType.IsGenericType)
            {
                if (outputType.GenericTypeArguments.Length != 2)
                {
                    ErrorContext.Error = new InvalidCastException("无法推断IDictionary的键值对应类型");
                    result = null;
                    return false;
                }
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
            IConvertor _keyConvertor;
            IConvertor _valueConvertor;
            Type _keyType;
            Type _valueType;
            public readonly IDictionary Dictionary;
            public ConvertHelper(Type type)
            {
                var genericTypes = type.GenericTypeArguments;
                if (genericTypes.Length == 0)
                {
                    _keyType = typeof(object);
                    _valueType = typeof(object);
                }
                else
                {
                    _keyType = genericTypes[0];
                    _valueType = genericTypes[1];
                }
                _keyConvertor = Convert3.GetConvertor(_keyType);
                _valueConvertor = Convert3.GetConvertor(_valueType);
                if (type.IsInterface)
                {
                    type = typeof(Dictionary<,>).MakeGenericType(_keyType, _valueType);
                }

                Dictionary = (IDictionary)Activator.CreateInstance(type);
            }

            public bool Add(object key, object value)
            {
                if (_keyConvertor == null || _valueConvertor == null)
                {
                    ErrorContext.ConvertorNotFound(
                        _keyConvertor == null ? _keyType : _valueType);
                    return false;
                }

                if (_keyConvertor.Try(key, _keyType, out key) == false)
                {
                    ErrorContext.Error = new NotImplementedException("Dictionary键转换失败");
                    return false;
                }
                if (_valueConvertor.Try(value, _valueType, out value) == false)
                {
                    ErrorContext.Error = new NotImplementedException("Dictionary值转换失败");
                    return false;
                }
                Dictionary.Add(key, value);
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

                if (_keyConvertor.Try(key, _keyType, out key) == false)
                {
                    ErrorContext.Error = new NotImplementedException("Dictionary键转换失败");
                    return false;
                }
                var value = getValue(param);
                if (_valueConvertor.Try(value, _valueType, out value) == false)
                {
                    ErrorContext.Error = new NotImplementedException("Dictionary值转换失败");
                    return false;
                }
                Dictionary.Add(key, value);
                return true;
            }
        }

        protected override bool Try(string input, Type outputType, out IDictionary result)
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
