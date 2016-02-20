using System;
using System.Collections.Generic;
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
            var o = Expression.Parameter(typeof(object), "o");
            var cast = Expression.Convert(o, property.DeclaringType);
            var p = Expression.Property(cast, property);
            Property = property;
            Name = property.Name;
            if (property.CanRead)
            {
                var ret = Expression.Convert(p, typeof(object));
                var get = Expression.Lambda<Func<object, object>>(ret, o);
                Get = get.Compile();
            }

            if (property.CanWrite)
            {
                var v = Expression.Parameter(typeof(object), "v");
                var val = Expression.Convert(v, property.PropertyType);
                var assign = Expression.MakeBinary(ExpressionType.Assign, p, val);
                var ret = Expression.Convert(assign, typeof(object));
                var set = Expression.Lambda<Action<object, object>>(ret, o, v);
                Set = set.Compile();
            }
        }
        public Func<object, object> Get { get; }
        public Action<object, object> Set { get; }
        public PropertyInfo Property { get; }
        public string Name { get; }
    }
}
