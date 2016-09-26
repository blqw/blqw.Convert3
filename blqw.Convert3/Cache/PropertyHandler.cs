using blqw.IOC;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary>
    /// 用于操作属性的Get和Set
    /// </summary>
    class PropertyHandler
    {
        public PropertyHandler(PropertyInfo property)
        {
            Property = property;
            PropertyType = property.PropertyType;
            Name = property.Name;
            Get = ComponentServices.GetGeter(property);
            Set = ComponentServices.GetSeter(property);
        }

        public Type PropertyType { get; }

        public Func<object, object> Get { get; }
        public Action<object, object> Set { get; }
        public PropertyInfo Property { get; }
        public string Name { get; }

        public bool SetValue(ConvertContext context, object target, object value)
        {
            if (Set == null)
            {
                Error.Add(new NotSupportedException($"{Property.ReflectedType}.{Property.Name}属性没有set"));
                return false;
            }
            bool b;
            var v = context.Get(PropertyType).ChangeType(context, value, PropertyType, out b);
            if (b == false)
            {
                Error.Add(new NotSupportedException($"{Property.ReflectedType}属性{Property.Name}赋值失败"));
                return false;
            }
            try
            {
                Set(target, v);
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
