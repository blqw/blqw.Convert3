using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Convert3Component
{
    class Component
    {
        private static bool IsInitialized { get; } = Initialize();

        private static bool Initialize()
        {
            MEF.Import(typeof(Component));
            return true;
        }
        
        /// <summary> 用于将Json字符串转为实体对象的方法
        /// </summary>
        [Import("ToJsonObject")]
        public readonly static Func<Type, string, object> ToJsonObject;

        /// <summary> 用于将Json字符串转为实体对象的方法
        /// </summary>
        [Import("ToJsonString")]
        public readonly static Func<object, string> ToJsonString;
    }
}
