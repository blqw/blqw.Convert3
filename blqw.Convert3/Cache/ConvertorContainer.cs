using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;
using System.ComponentModel.Composition;
using blqw.Convert3Component;
using System.Collections.Generic;

namespace blqw
{
    /// <summary> 
    /// 转换器容器
    /// </summary>
    public class ConvertorContainer
    {
        /// <summary> 标准的字典缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IConvertor> _cache 
            = new ConcurrentDictionary<Type, IConvertor>();

        class Import
        {
            [ImportMany(typeof(IConvertor))]
            public List<IConvertor> Convertors;
        }

        /// <summary>
        /// 加载转换器
        /// </summary>
        public void Load()
        {
            var import = new Import();
            MEFPart.Import(import);
            import.Convertors.ForEach(it => it.Initialize());
            foreach (var conv in import.Convertors)
            {
                _cache.TryAdd(conv.OutputType, conv);
            }
        }


        /// <summary> 获取转换器
        /// </summary>
        /// <param name="key">缓存键</param>
        public IConvertor Get(Type key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            IConvertor conv;
            if (_cache.TryGetValue(key, out conv))
            {
                return conv;
            }

            conv = OnOptimal(key, MatchConvertor(key))?.GetConvertor(key);
            if (conv == null)
            {
                throw new NotSupportedException("无法获取与当前类型匹配的转换器");
            }
            _cache.TryAdd(key, conv);
            return conv;
        }

        private IConvertor OnOptimal(Type outputType, IEnumerable<IConvertor> convertors)
        {
            return convertors?.FirstOrDefault();
        }



        /// <summary> 泛型缓存
        /// </summary>
        class GenericCache<Key>
        {
            public static IConvertor<Key> Convertor;
        }

        /// <summary> 获取缓存值
        /// </summary>
        /// <typeparam name="Key">缓存键的类型</typeparam>
        public IConvertor<Key> Get<Key>()
        {
            return GenericCache<Key>.Convertor
                ?? (GenericCache<Key>.Convertor = Get(typeof(Key)) as IConvertor<Key>);
        }

        private IEnumerable<IConvertor> MatchConvertor(Type key)
        {
            IConvertor conv;
            var baseType = key.BaseType;
            while (baseType != null)
            {
                if (_cache.TryGetValue(baseType, out conv))
                {
                    yield return conv;
                }
                else if (baseType.IsGenericType && baseType.IsGenericTypeDefinition == false)
                {
                    if (_cache.TryGetValue(baseType.GetGenericTypeDefinition(), out conv))
                    {
                        yield return conv;
                    }
                }
                baseType = baseType.BaseType;
            }
            foreach (var @interface in key.GetInterfaces())
            {
                if (_cache.TryGetValue(@interface, out conv))
                {
                    yield return conv;
                }
                else if (@interface.IsGenericType && @interface.IsGenericTypeDefinition == false)
                {
                    if (_cache.TryGetValue(@interface.GetGenericTypeDefinition(), out conv))
                    {
                        yield return conv;
                    }
                }
            }
        }

    }
}
