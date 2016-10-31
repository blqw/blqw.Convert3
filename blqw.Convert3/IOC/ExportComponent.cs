using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace blqw.IOC
{
    internal static class ExportComponent
    {
        public const int PRIORITY = 108;


        /// <summary>
        /// 用于数据转换的输出插件
        /// </summary>
        [Export("Convert")]
        [ExportMetadata("Priority", PRIORITY)]
        public static object Convert(object input, Type convertType, bool throwError)
            => throwError ? input.ChangeType(convertType) : input.ChangeType(convertType, null);

        /// <summary>
        /// 获取转换器
        /// </summary>
        [Export("GetConverter")]
        [ExportMetadata("Priority", PRIORITY)]
        public static IFormatterConverter GetConverter(Type convertType, bool throwError)
            => new DirectConverter(convertType, throwError);

        /// <summary>
        /// 获取动态类型
        /// </summary>
        [Export("GetDynamic")]
        [ExportMetadata("Priority", PRIORITY)]
        public static dynamic GetDynamic(object obj) 
            => obj.ToDynamic();
    }
}