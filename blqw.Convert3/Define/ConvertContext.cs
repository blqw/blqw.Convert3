using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.Converts;

namespace blqw
{
    /// <summary>
    /// 类型转换中所使用的参数
    /// </summary>
    public class ConvertContext
    {
        /// <summary>
        /// 转换器的提供程序
        /// </summary>
        public IServiceProvider ConvertorProvider { get; set; }
        
        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public IConvertor Get(Type outputType) => (IConvertor)ConvertorProvider?.GetService(outputType) ?? ConvertorServices.Container?.GetConvertor(outputType);

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IConvertor<T> Get<T>() => (IConvertor<T>)ConvertorProvider?.GetService(typeof(T)) ?? ConvertorServices.Container?.GetConvertor<T>();

        public void AddException(string message)
        {
            
        }
    }
}
