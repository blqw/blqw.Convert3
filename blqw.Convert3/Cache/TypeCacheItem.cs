using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 缓存项
    /// </summary>
    class TypeCacheItem
    {
        /// <summary> 新建CacheItem对象
        /// </summary>
        /// <param name="outputType">输出类型</param>
        /// <param name="conv">转换器</param>
        /// <returns></returns>
        public static TypeCacheItem New(Type outputType, IConvertor conv)
        {
            return new TypeCacheItem(outputType, conv);//?? TypeCache.Get<object>().Convertor);
        }
        protected TypeCacheItem(Type outputType, IConvertor conv)
        {
            Convertor = conv;
            Type = outputType;
            TypeName = CType.GetFriendlyName(outputType);
            if (Type.IsValueType
                && Type.IsGenericTypeDefinition == false
                && Nullable.GetUnderlyingType(Type) == null)
            {
                DefaultValue = Activator.CreateInstance(Type);
            }
        }
        /// <summary> 默认值
        /// </summary>
        public readonly object DefaultValue;
        /// <summary> 输出类型
        /// </summary>
        public readonly Type Type;
        /// <summary> 类型名称缓存
        /// </summary>
        public readonly string TypeName;
        /// <summary> 转换器
        /// </summary>
        public readonly IConvertor Convertor;
    }
}
