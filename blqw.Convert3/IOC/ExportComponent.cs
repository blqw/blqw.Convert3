using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using blqw;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace blqw.IOC
{

    static class ExportComponent
    {
        public const int PRIORITY = 106;


        static readonly JavaScriptSerializer JSON = new JavaScriptSerializer();
        /// <summary> 
        /// 用于将Json字符串转为实体对象的方法
        /// </summary>
        [Export("ToJsonObject")]
        public static object ToJsonObject(Type type, string json)
        {
            return JSON.Deserialize(json, type);
        }

        /// <summary>
        /// 用于将Json字符串转为实体对象的方法
        /// </summary>
        [Export("ToJsonString")]
        public static string ToJsonString(object obj)
        {
            return JSON.Serialize(obj);
        }

        /// <summary> 用于数据转换的输出插件
        /// </summary>
        [Export("Convert")]
        [ExportMetadata("Priority", PRIORITY)]
        public static object Convert(object input, Type convertType, bool throwError)
        {
            if (throwError)
            {
                return input.ChangeType(convertType);
            }
            return input.ChangeType(convertType, null);
        }

        /// <summary> 获取转换器
        /// </summary>
        [Export("GetConverter")]
        [ExportMetadata("Priority", PRIORITY)]
        public static IFormatterConverter GetConverter(Type convertType, bool throwError)
        {
            return new DirectConverter(convertType, throwError);
        }

        /// <summary> 获取动态类型
        /// </summary>
        [Export("GetDynamic")]
        [ExportMetadata("Priority", PRIORITY)]
        public static dynamic GetDynamic(object obj)
        {
            return obj.ToDynamic();
        }

        static object Convert1(this IConvertor conv, object obj)
        {
            using (Error.Contract())
            {
                bool b;
                var result = conv.ChangeType(obj, conv.OutputType, out b);
                if (b == false)
                {
                    Error.CastFail(obj, conv.OutputType);
                    Error.ThrowIfHaveError();
                }
                return result;
            }
        }

        static object Convert2(this IConvertor conv, object obj)
        {
            bool b;
            var result = conv.ChangeType(obj, conv.OutputType, out b);
            if (b == false)
            {
                return Convert3.GetDefaultValue(conv.OutputType);
            }
            return result;
        }

    }
}
