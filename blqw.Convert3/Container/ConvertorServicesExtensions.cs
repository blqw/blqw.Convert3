using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.IOC;

namespace blqw
{
    public static class ConvertorServicesExtensions
    {
        private static class Cache<T>
        {
            public static ServiceItem Wapper { get; set; }
        }

        private static ServiceItem _NullWapper;

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <exception cref="OverflowException"> 字典中已包含元素的最大数目 (<see cref="F:System.Int32.MaxValue" />)。 </exception>
        public static IConvertor<T> GetConvertor<T>(this ConvertorServices container)
        {
            if (object.ReferenceEquals(ConvertorServices.Container, container))
            {
                return (IConvertor<T>)(Cache<T>.Wapper ?? (Cache<T>.Wapper = container.GetServiceItem(typeof(T))))?.Value;
            }
            return (IConvertor<T>)container.GetServiceItem(typeof(T))?.Value;
        }

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <param name="container"></param>
        /// <param name="type"> </param>
        /// <returns></returns>
        /// <exception cref="OverflowException"> 字典中已包含元素的最大数目 (<see cref="F:System.Int32.MaxValue" />)。 </exception>
        public static IConvertor GetConvertor(this ConvertorServices container, Type type) => (IConvertor)container.GetService(type);
        
    }
}
