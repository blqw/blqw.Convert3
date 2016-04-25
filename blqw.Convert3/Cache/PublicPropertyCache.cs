using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;

namespace blqw
{
    /// <summary>
    /// 公开属性缓存
    /// </summary>
    static class PublicPropertyCache
    {
        static readonly ConcurrentDictionary<PropertyInfo, PropertyHandler> _PropertyCache = new ConcurrentDictionary<PropertyInfo, PropertyHandler>();

        static readonly ConcurrentDictionary<Type, PropertyHandler[]> _TypeCache = new ConcurrentDictionary<Type, PropertyHandler[]>();

        static readonly PropertyHandler[] _Empty = new PropertyHandler[0];

        /// <summary>
        /// 根据类型获取操作属性的对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyHandler[] GetByType(Type type)
        {
            if (type == null)
            {
                return null;
            }
            PropertyHandler[] result;
            if (_TypeCache.TryGetValue(type, out result))
            {
                return result;
            }

            var ps = type.GetProperties();
            var length = ps.Length;
            if (length == 0)
            {
                _TypeCache.TryAdd(type, _Empty);
                return _Empty;
            }
            result = new PropertyHandler[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = GetPropertyHandler(ps[i]);
            }
            _TypeCache.TryAdd(type, result);
            return result;
        }

        internal static PropertyHandler GetPropertyHandler(this PropertyInfo property)
        {
            if (property == null)
            {
                return null;
            }
            PropertyHandler handler;
            if (_PropertyCache.TryGetValue(property,out handler))
            {
                return handler;
            }
            handler = new PropertyHandler(property);
            _PropertyCache.TryAdd(property, handler);
            return handler;
        }
    }
}
