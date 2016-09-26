using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary>
    /// 类型转换中所使用的参数
    /// </summary>
    public sealed class ConvertArgs
    {
        /// <summary>
        /// 转换器的提供程序
        /// </summary>
        public IServiceProvider ConvertorProvider { get; private set; }
        
        /// <summary>
        /// 需要转换的类型
        /// </summary>
        public Type OutputType { get; private set; }


        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <param name="outputType"></param>
        /// <returns></returns>
        /// <exception cref="OverflowException"> 字典中已包含元素的最大数目 (<see cref="F:System.Int32.MaxValue" />)。 </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="outputType" /> is <see langword="null" />. </exception>
        public IConvertor Get(Type outputType) => (IConvertor)ConvertorProvider?.GetService(outputType) ?? ConvertorServices.Container?.GetConvertor(outputType);

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="OverflowException"> 匹配插件数量超过字典的最大容量 (<see cref="int.MaxValue" />)。 </exception>
        public IConvertor<T> Get<T>() => (IConvertor<T>)ConvertorProvider?.GetService(typeof(T)) ?? ConvertorServices.Container?.GetConvertor<T>();

    }
}
