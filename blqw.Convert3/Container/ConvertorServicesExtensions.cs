using System;
using blqw.IOC;

namespace blqw
{
    /// <summary>
    /// 转换器容器扩展方法
    /// </summary>
    /// <remarks>为了不破坏 <see cref="ConvertorServices"/> 的独立性</remarks>
    public static class ConvertorServicesExtensions
    {
        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <typeparam name="T"> 转换服务类型 </typeparam>
        /// <param name="container"> 服务容器 </param>
        /// <returns> </returns>
        /// <exception cref="OverflowException"> 字典中已包含元素的最大数目 (<see cref="F:System.Int32.MaxValue" />)。 </exception>
        public static IConvertor<T> GetConvertor<T>(this ConvertorServices container)
        {
            if (ReferenceEquals(ConvertorServices.Container, container))
            {
                return (IConvertor<T>) (Cache<T>.Service ?? (Cache<T>.Service = container.GetServiceItem(typeof(T))))?.Value;
            }
            return (IConvertor<T>) container.GetServiceItem(typeof(T))?.Value;
        }

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <param name="container"> 服务容器 </param>
        /// <param name="type"> 转换服务类型 </param>
        /// <returns> </returns>
        /// <exception cref="OverflowException"> 字典中已包含元素的最大数目 (<see cref="F:System.Int32.MaxValue" />)。 </exception>
        public static IConvertor GetConvertor(this ConvertorServices container, Type type) 
            => (IConvertor) container.GetService(type);

        /// <summary>
        /// 泛型缓存
        /// </summary>
        /// <typeparam name="T">  </typeparam>
        // ReSharper disable once UnusedTypeParameter
        private static class Cache<T>
        {
            /// <summary>
            /// 缓存服务项
            /// </summary>
            public static ServiceItem Service { get; set; }
        }
    }
}