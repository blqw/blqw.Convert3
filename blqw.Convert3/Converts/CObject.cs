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

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CObject : AdvancedConvertor<object>, IIgnoreInherit
    {
        protected override bool Try(object input, Type outputType, out object result)
        {
            if (input == null)
            {
                result = null;
                return true;
            }
            var nv = input as NameValueCollection;
            if (nv != null)
            {
                var obj = new SetObjectProperty(outputType);
                foreach (string name in nv)
                {
                    if (obj.Set(name, nv[name]) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = obj.Instance;
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
                var obj = new SetObjectProperty(outputType);
                var cols = row.Table.Columns;
                foreach (DataColumn col in cols)
                {
                    if (obj.Set(col.ColumnName, row[col]) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = obj.Instance;
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
                var obj = new SetObjectProperty(outputType);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (obj.Set(reader.GetName(i), reader.GetValue, i) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = obj.Instance;
                return true;
            }

            var dict = input as IDictionary;
            if (dict != null)
            {
                var obj = new SetObjectProperty(outputType);
                foreach (DictionaryEntry item in dict)
                {
                    var name = item.Key as string;
                    if (name != null && obj.Set(name, item.Value) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = obj.Instance;
                return true;
            }

            var ps = PublicPropertyCache.GetByType(input.GetType());
            if (ps.Length > 0)
            {
                var obj = new SetObjectProperty(outputType);
                foreach (var p in ps)
                {
                    if (p.Get != null && obj.Set(p.Name, p.Get, input) == false)
                    {
                        result = null;
                        return false;
                    }
                }
                result = obj.Instance;
                return true;
            }
            ErrorContext.CastFail(input, outputType);
            result = null;
            return false;
        }

        struct SetObjectProperty
        {
            PropertyHandler[] _properties;
            int _propertyCount;
            public readonly object Instance;
            public SetObjectProperty(Type type)
            {
                _properties = PublicPropertyCache.GetByType(type);
                _propertyCount = _properties.Length;
                Instance = Activator.CreateInstance(type);
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
                var p = GetProperty(name);
                if (p != null && p.Set != null)
                {
                    if (Convert3.TryChangedType(value, p.Property.PropertyType, out value))
                    {
                        p.Set(Instance, value);
                        return true;
                    }
                    ErrorContext.Error = WriteFail(p.Property);
                    return false;
                }
                return true;
            }

            public bool Set<P>(string name, Func<P, object> getValue, P param)
            {
                var p = GetProperty(name);
                if (p != null && p.Set != null)
                {
                    var value = getValue(param);
                    if (Convert3.TryChangedType(value, p.Property.PropertyType, out value))
                    {
                        p.Set(Instance, value);
                        return true;
                    }
                    ErrorContext.Error = WriteFail(p.Property);
                    return false;
                }
                return true;
            }

            private NotImplementedException WriteFail(PropertyInfo p)
            {
                return new NotImplementedException(CType.GetFriendlyName(p.ReflectedType) + "." + p.Name + " 赋值失败");
            }
        }

        protected override bool Try(string input, Type outputType, out object result)
        {
            try
            {
                var type = Type.GetType(input, false, true);
                if (type == null)
                {
                    ErrorContext.Error = new TypeLoadException("没有找到名为 " + input + " 的类型");
                    result = null;
                    return false;
                }
                else if (outputType.IsAssignableFrom(type) == false)
                {
                    ErrorContext.CastFail(type, outputType);
                    result = null;
                    return false;
                }

                outputType = type;
                result = Activator.CreateInstance(type, true);
                return true;
            }
            catch (FileLoadException ex)
            {
                ErrorContext.CastFail(input, outputType);
            }
            catch (TargetInvocationException ex)
            {
                ErrorContext.Error = ex;
                Trace.TraceInformation(CType.GetFriendlyName(outputType) + ",初始化失败:" + ex.Message);
            }
            catch (Exception ex)
            {
                ErrorContext.Error = ex;
            }
            result = null;
            return false;
        }

        static IConvertor<object> _convertor = new CObject();

        /// <summary> 尝试将指定对象转换为指定类型的值。返回是否转换成功
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="result">如果转换成功,则包含转换后的对象,否则为default(T)</param>
        public static bool TryTo<T>(object input, Type outputType, out T result)
        {
            object r;
            if (_convertor.Try(input, outputType, out r))
            {
                result = (T)r;
                return true;
            }
            result = default(T);
            return false;
        }

        /// <summary> 尝试将指定对象转换为指定类型的值。返回是否转换成功
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="result">如果转换成功,则包含转换后的对象,否则为default(T)</param>
        public static bool TryTo<T>(string input, Type outputType, out T result)
        {
            object r;
            if (_convertor.Try(input, outputType, out r))
            {
                result = (T)r;
                return true;
            }
            result = default(T);
            return false;
        }
    }
}
