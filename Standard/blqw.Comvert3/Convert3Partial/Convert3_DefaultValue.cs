using blqw.Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    public partial class Convert3
    {
        /// <summary>
        /// 获取一个类型的默认值
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        [Export("GetDefaultValue")]
        [ExportMetadata("Priority", PRIORITY)]
        public static object GetDefaultValue(this Type type)
        {
            if ((type == null)
                || (type.IsValueType == false) //不是值类型
                || type.IsGenericTypeDefinition //泛型定义类型
                || (Nullable.GetUnderlyingType(type) != null)) //可空值类型
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }
    }
}
