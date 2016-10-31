using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace blqw
{
    /// <summary>
    /// 公开属性缓存
    /// </summary>
    internal static class PublicPropertyCache
    {
        /// <summary>
        /// 属性缓存
        /// </summary>
        private static readonly ConcurrentDictionary<PropertyInfo, PropertyHandler> _PropertyCache =
            new ConcurrentDictionary<PropertyInfo, PropertyHandler>();

        /// <summary>
        /// 类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyHandler[]> _TypeCache =
            new ConcurrentDictionary<Type, PropertyHandler[]>();

        /// <summary>
        /// 表示一个空属性集合
        /// </summary>
        private static readonly PropertyHandler[] _Empty = new PropertyHandler[0];

        /// <summary>
        /// 根据类型获取操作属性的对象集合
        /// </summary>
        /// <param name="type"> 需要获取属性的类型 </param>
        /// <returns> </returns>
        public static PropertyHandler[] GetByType(Type type)
            => type == null ? null : _TypeCache.GetOrAdd(type, Create);

        /// <summary>
        /// 根据类型创建一个操作属性的对象集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static PropertyHandler[] Create(Type type)
        {
            var ps = type.GetProperties();
            var length = ps.Length;
            if (length == 0)
            {
                _TypeCache.TryAdd(type, _Empty);
                return _Empty;
            }
            var result = new PropertyHandler[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = GetPropertyHandler(ps[i]);
            }
            return result;
        }

        /// <summary>
        /// 根据属性值获取属性操作对象
        /// </summary>
        /// <param name="property"> 属性 </param>
        /// <returns></returns>
        internal static PropertyHandler GetPropertyHandler(this PropertyInfo property) 
            => property == null ? null : _PropertyCache.GetOrAdd(property, PropertyHandler.Create);
    }
}