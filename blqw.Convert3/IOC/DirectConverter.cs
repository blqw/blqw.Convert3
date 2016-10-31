using System;

namespace blqw.IOC
{
    /// <summary>
    /// 定向转换器
    /// </summary>
    internal class DirectConverter : ObjectConverter
    {
        private readonly Type _outputType;
        private readonly bool _throwError;
        private IConvertor _convertor;

        /// <summary>
        /// 初始化转换器实例
        /// </summary>
        /// <param name="outputType"> 待转换类型 </param>
        /// <param name="throwError"> 是否抛出异常 </param>
        public DirectConverter(Type outputType, bool throwError)
        {
            _outputType = outputType;
            _throwError = throwError;
        }

        private IConvertor Convertor => _convertor ?? (_convertor = ConvertorServices.Container.GetConvertor(_outputType));

        /// <summary>
        /// 将值转换为给定的 <see cref="T:System.Type" />。
        /// </summary>
        /// <returns> 转换后的 <paramref name="value" />。 </returns>
        /// <param name="value"> 要转换的对象。 </param>
        /// <param name="type">
        /// <paramref name="value" /> 将转换成的 <see cref="T:System.Type" />。
        /// </param>
        public override object Convert(object value, Type type)
        {
            using (var context = new ConvertContext())
            {
                bool b;
                var output = Convertor.ChangeType(context, value, type, out b);
                if (_throwError && (b == false))
                {
                    context.ThrowIfHaveError();
                }
                return output;
            }
        }

        /// <summary>
        /// 将值转换为给定的 <see cref="T:System.Type" />。
        /// </summary>
        /// <returns> 转换后的 <paramref name="value" />。 </returns>
        /// <param name="value"> 要转换的对象。 </param>
        public override T Convert<T>(object value)
        {
            using (var context = new ConvertContext())
            {
                bool b;
                var output = Convertor.ChangeType(context, value, typeof(T), out b);
                if (_throwError && (b == false))
                {
                    context.ThrowIfHaveError();
                }
                return (T) output;
            }
        }
    }
}