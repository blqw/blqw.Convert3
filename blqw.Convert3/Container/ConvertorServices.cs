using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using blqw.IOC;

namespace blqw
{
    /// <summary>
    /// 转换器容器
    /// </summary>
    public sealed class ConvertorServices : ServiceContainer
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <exception cref="OverflowException"> 匹配插件数量超过字典的最大容量 (<see cref="int.MaxValue" />)。 </exception>
        private ConvertorServices()
            : base(null, typeof(IConvertor), new TypeComparer())
        {
        }

        /// <summary>
        /// 容器
        /// </summary>
        /// <exception cref="OverflowException"> 匹配插件数量超过字典的最大容量 (<see cref="int.MaxValue" />)。 </exception>
        public static ConvertorServices Container { get; } = new ConvertorServices();

        /// <summary>
        /// 获取插件的服务类型 <see cref="T:System.Type" />, 默认 <code>plugIn.GetMetadata&lt;Type&gt;("ServiceType")</code>
        /// </summary>
        /// <param name="plugIn"> 插件 </param>
        /// <param name="value"> 插件值 </param>
        /// <returns> </returns>
        protected override Type GetServiceType(PlugIn plugIn, object value) => (value as IConvertor)?.OutputType;

        public override ServiceItem GetServiceItem(Type serviceType)
        {
            if (serviceType != null)
            {
                if (serviceType.IsGenericTypeDefinition)
                {
                    throw new ArgumentOutOfRangeException($"无法为泛型定义类型`{serviceType}`提供转换器");
                }
                if (serviceType.IsAbstract && serviceType.IsSealed)
                {
                    throw new ArgumentOutOfRangeException($"无法为静态类型`{serviceType}`提供转换器");
                }
            }
            return base.GetServiceItem(serviceType);
        }

        /// <summary>
        /// 类型比较器
        /// </summary>
        private sealed class TypeComparer : IComparer<Type>
        {
            /// <summary>
            /// 部分类型优先级调整
            /// </summary>
            private static readonly Dictionary<Type, int> _Priorities = new Dictionary<Type, int>
            {
                [typeof(IObjectReference)] = 400,
                [typeof(IFormatProvider)] = 300,
                [typeof(IDictionary<,>)] = 200,
                [typeof(IDictionary)] = 199,
                [typeof(IEnumerable<>)] = 99,
                [typeof(IEnumerable)] = 98,
                [typeof(IEnumerator<>)] = 97,
                [typeof(IEnumerator)] = 96,
                [typeof(DynamicObject)] = 95
            };

            /// <summary>
            /// 比较两个对象并返回一个值，该值指示一个对象小于、等于还是大于另一个对象。
            /// </summary>
            /// <returns>
            /// 一个有符号整数，指示 <paramref name="x" /> 与 <paramref name="y" /> 的相对值，如下表所示。值含义小于零<paramref name="x" /> 小于
            /// <paramref name="y" />。零<paramref name="x" /> 等于 <paramref name="y" />。大于零<paramref name="x" /> 大于 <paramref name="y" />
            /// 。
            /// </returns>
            /// <param name="x"> 要比较的第一个对象。 </param>
            /// <param name="y"> 要比较的第二个对象。 </param>
            public int Compare(Type x, Type y) => GetPriority(x).CompareTo(GetPriority(y));

            /// <summary>
            /// 获取类型的优先级
            /// </summary>
            /// <param name="type"> </param>
            /// <returns> </returns>
            private static int GetPriority(Type type)
            {
                if (type == null)
                {
                    return 0;
                }
                int i;
                if (type.IsGenericType)
                {
                    type = type.GetGenericTypeDefinition() ?? type;
                }
                return _Priorities.TryGetValue(type, out i) ? i : 100;
            }


        }
    }
}