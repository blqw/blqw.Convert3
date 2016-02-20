using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;

namespace blqw
{
    /// <summary> 用于处于Type类型为Key时的缓存
    /// </summary>
    class TypeCache
    {
        /// <summary> 标准的字典缓存
        /// </summary>
        private readonly Dictionary<Type, TypeCacheItem> _cache = new Dictionary<Type, TypeCacheItem>();

        /// <summary> 泛型缓存
        /// </summary>
        class GenericCache<Key>
        {
            public readonly static TypeCacheItem Item;
            public readonly static bool IsReadied;
            private Key x;
        }

        /// <summary> 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="item">缓存值</param>
        public void Set(Type key, TypeCacheItem item)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (IsInitialized)
            {
                item.Convertor.Initialize();
            }
            if (key.IsGenericTypeDefinition) //如果是泛型定义类型 就不能加入泛型缓存
            {
                lock (_cache)
                {
                    _cache[key] = item;
                }
                return;
            }
            var gc = typeof(GenericCache<>).MakeGenericType(key);
            var fieid1 = gc.GetField("Item", BindingFlags.Public | BindingFlags.Static);
            var fieid2 = gc.GetField("IsReadied", BindingFlags.Public | BindingFlags.Static);
            lock (_cache)
            {
                _cache[key] = item;
                fieid1.SetValue(null, item);
                fieid2.SetValue(null, true);
            }
        }

        /// <summary> 获取缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        public TypeCacheItem Get(Type key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            TypeCacheItem item;
            if (_cache.TryGetValue(key, out item))
            {
                return item;
            }
            return default(TypeCacheItem);
        }

        /// <summary> 获取缓存值
        /// </summary>
        /// <typeparam name="Key">缓存键的类型</typeparam>
        public TypeCacheItem Get<Key>()
        {
            return GenericCache<Key>.Item;
        }

        /// <summary> 获取缓存值,如果指定缓存不存在则使用create参数获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="create">用于创建缓存项委托</param>
        public TypeCacheItem GetOrCreate(Type key, Func<TypeCacheItem> create)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            TypeCacheItem item;
            if (_cache.TryGetValue(key, out item))
            {
                return item;
            }
            if (create == null)
            {
                throw new ArgumentNullException("create");
            }
            item = create();
            lock (_cache)
            {
                if (_cache.ContainsKey(key))
                {
                    return _cache[key];
                }
                Set(key, item);
                return item;
            }
        }

        /// <summary> 获取缓存值,如果指定缓存不存在则使用create参数获取缓存
        /// </summary>
        /// <typeparam name="Key">缓存键的类型</typeparam>
        /// <param name="create">用于创建缓存项委托</param>
        public TypeCacheItem GetOrCreate<Key>(Func<TypeCacheItem> create)
        {
            if (GenericCache<Key>.IsReadied)
            {
                return GenericCache<Key>.Item;
            }
            if (create == null)
            {
                throw new ArgumentNullException("create");
            }
            var item = create();
            lock (_cache)
            {
                if (GenericCache<Key>.IsReadied)
                {
                    return GenericCache<Key>.Item;
                }
                Set(typeof(Key), item);
                return item;
            }
        }

        private static bool IsInitialized;
        internal void Initialize()
        {
            IsInitialized = true;
            foreach (var item in _cache.Values.ToList())
            {
                item.Convertor.Initialize();
            }
        }
    }
}
