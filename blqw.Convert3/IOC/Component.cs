using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace blqw.Convert3Component
{
    class Component
    {
        private static bool IsInitialized { get; } = Initialize();

        private static bool Initialize()
        {
            MEFPart.Import(typeof(Component));
            return true;
        }


        static readonly JavaScriptSerializer JSON = new JavaScriptSerializer();
        /// <summary> 用于将Json字符串转为实体对象的方法
        /// </summary>
        [Import("ToJsonObject")]
        public readonly static Func<Type, string, object> ToJsonObject =
            delegate (Type type, string json)
            {
                return JSON.Deserialize(json, type);
            };

        /// <summary> 用于将Json字符串转为实体对象的方法
        /// </summary>
        [Import("ToJsonString")]
        public readonly static Func<object, string> ToJsonString =
            delegate (object obj)
            {
                return JSON.Serialize(obj);
            };
    }
}
