using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using blqw.IOC;

namespace blqw
{
    /// <summary> 
    /// 转换器容器
    /// </summary>
    public sealed class ConvertorContainer
    {
        static ConvertorContainer()
        {
            Default = new ConvertorContainer();
            Default.ReLoad();
        }
        public static readonly ConvertorContainer Default;


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
            MEFLite.Import(import);
            foreach (var conv in import.Convertors)
            {
                var get = _cache.GetOrAdd(conv.OutputType, conv);
                if (ReferenceEquals(get, conv) == false && conv.Priority > get.Priority)
                {
                    _cache.TryUpdate(conv.OutputType, conv, get);
                }
            }
            import.Convertors.ForEach(it => it.Initialize());
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
                Error.Add(new NotSupportedException($"无法获取与 {key} 类型匹配的转换器"));
                return null;
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
            return convertors?.Where(it => it.OutputType != typeof(object)).FirstOrDefault()
                    ?? convertors.FirstOrDefault();
        }

        /// <summary> 泛型缓存
        /// </summary>
        class GenericCache<Key>
        {
            static IConvertor<Key> _Convertor;
            public static IConvertor<Key> Convertor
            {
                get
                {
                    return _Convertor
                    ?? (_Convertor = Default.Get(typeof(Key)) as IConvertor<Key>);
                }
            }
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
            var baseType = key;
            while (baseType != null)
            {
                var conv = TryGetConvertor(baseType);
                if (conv != null)
                {
                    yield return conv;
                }
                baseType = baseType.BaseType;
            }
            foreach (var @interface in key.GetInterfaces())
            {
                var conv = TryGetConvertor(@interface);
                if (conv != null)
                {
                    yield return conv;
                }
            }
        }

        private IConvertor TryGetConvertor(Type type)
        {
            IConvertor conv;
            if (_cache.TryGetValue(type, out conv))
            {
                return conv;
            }
            else if (type.IsGenericType && type.IsGenericTypeDefinition == false)
            {
                if (_cache.TryGetValue(type.GetGenericTypeDefinition(), out conv))
                {
                    return conv;
                }
            }
            return null;
        }

    }
}
