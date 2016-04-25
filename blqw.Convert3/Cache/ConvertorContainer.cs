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
    public sealed class ConvertorContainer
    {
        public static readonly ConvertorContainer Default = new ConvertorContainer();

        public static IConvertor<string> StringConvertor
        {
            get
            {
                return GenericCache<string>.Convertor;
            }
        }

        public static IConvertor<long> Int64Convertor
        {
            get
            {
                return GenericCache<long>.Convertor;
            }
        }

        public static IConvertor<ulong> UInt64Convertor
        {
            get
            {
                return GenericCache<ulong>.Convertor;
            }
        }

        private ConvertorContainer()
        {
            _cache = new ConcurrentDictionary<Type, IConvertor>();
            ReLoad();
        }

        /// <summary> 标准的字典缓存
        /// </summary>
        private readonly ConcurrentDictionary<Type, IConvertor> _cache;

        class Import
        {
            [ImportMany(typeof(IConvertor))]
            public List<IConvertor> Convertors;
        }

        /// <summary>
        /// 加载转换器
        /// </summary>
        internal void ReLoad()
        {
            _cache.Clear();
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

            conv = OnOptimal(MatchConvertor(key), key)?.GetConvertor(key);
            if (conv == null)
            {
                throw new NotSupportedException("无法获取与当前类型匹配的转换器");
            }
            conv.Initialize();
            _cache.TryAdd(key, conv);
            return conv;
        }

        /// <summary>
        /// 选出最符合输出类型的转换器
        /// </summary>
        /// <param name="convertors"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        private IConvertor OnOptimal(IEnumerable<IConvertor> convertors, Type outputType)
        {
            return convertors?.FirstOrDefault();
        }

        /// <summary> 泛型缓存
        /// </summary>
        class GenericCache<Key>
        {
            public static IConvertor<Key> Convertor = Default.Get(typeof(Key)) as IConvertor<Key>;
        }

        /// <summary> 获取缓存值
        /// </summary>
        /// <typeparam name="Key">缓存键的类型</typeparam>
        public IConvertor<Key> Get<Key>()
        {
            if (ReferenceEquals(this, Default))
            {
                return GenericCache<Key>.Convertor;
            }
            return Get(typeof(Key)) as IConvertor<Key>;
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
