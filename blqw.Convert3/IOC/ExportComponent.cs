using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using blqw;
using System.ComponentModel;

namespace blqw.Convert3Component
{

    static class ExportComponent
    {
        /// <summary> 用于数据转换的输出插件
        /// </summary>
        [Export("Convert")]
        [ExportMetadata("Priority", 100)]
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
        [ExportMetadata("Priority", 100)]
        public static IFormatterConverter GetConverter(Type convertType, bool throwError)
        {
            return new DirectConverter(convertType, throwError);
        }

        /// <summary> 获取动态类型
        /// </summary>
        [Export("GetDynamic")]
        [ExportMetadata("Priority", 100)]
        public static dynamic GetDynamic(object obj)
        {
            return obj.ToDynamic();
        }
        
        static object Convert1(this IConvertor conv, object obj)
        {
            using (ErrorContext.Callin())
            {
                object output;
                if (conv.Try(obj, conv.OutputType, out output) == false)
                {
                    Convert3.ThrowError(obj, conv.OutputType);
                }
                return output;
            }
        }

        static object Convert2(this IConvertor conv, object obj)
        {
            if (conv.Try(obj, conv.OutputType, out obj))
            {
                return obj;
            }
            return Convert3.GetDefaultValue(conv.OutputType);
        }

    }
}
