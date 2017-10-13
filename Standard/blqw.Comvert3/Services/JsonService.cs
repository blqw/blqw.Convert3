using blqw.Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Services
{
    /// <summary>
    /// Json 转换服务
    /// </summary>
    public class JsonService
    {
        public JsonService(IServiceProvider provider)
        {
            blqw.Autofac.PartContainer
        }
        /// <summary>
        /// 用于将Json字符串转为实体对象的方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        //[Import("ToJsonObject")]
        object ToJsonObject(Type type, string jsonString);

        /// <summary>
        /// 用于将Json字符串转为实体对象的方法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //[Import("ToJsonString")]
        string ToJsonString(object value);
    }
}
