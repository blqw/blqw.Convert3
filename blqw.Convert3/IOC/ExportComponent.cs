using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using blqw;
using System.ComponentModel;

namespace blqw.IOC
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
