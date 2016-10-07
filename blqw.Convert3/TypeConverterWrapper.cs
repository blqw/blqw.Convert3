using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.Converts;

namespace blqw
{
    sealed class TypeConverterWrapper: TypeConverter
    {
        private readonly IConvertor _convertor;

        public TypeConverterWrapper(IConvertor convertor)
        {
            if (convertor == null)
            {
                throw new ArgumentNullException(nameof(convertor));
            }
            _convertor = convertor;
        }

        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
        /// </summary>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        /// <param name="sourceType">一个 <see cref="T:System.Type" />，表示要转换的类型。</param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;

        /// <summary>
        /// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型。</summary>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        /// <param name="destinationType">一个 <see cref="T:System.Type" />，表示要转换到的类型。</param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
            => _convertor.OutputType.IsAssignableFrom(destinationType);

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型。
        /// </summary>
        /// <returns>表示转换的 value 的 <see cref="T:System.Object" />。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        /// <param name="culture">用作当前区域性的 <see cref="T:System.Globalization.CultureInfo" />。</param>
        /// <param name="value">要转换的 <see cref="T:System.Object" />。</param>
        /// <exception cref="InvalidCastException"> 转换失败 </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            using (var ctx = new ConvertContext())
            {
                bool success;
                var result = _convertor.ChangeType(ctx, value, _convertor.OutputType, out success); if (success == false)
                {
                    ctx.ThrowIfHaveError();
                }
                return result;
            }
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型。
        /// </summary>
        /// <returns>表示转换的 value 的 <see cref="T:System.Object" />。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        /// <param name="culture">
        /// <see cref="T:System.Globalization.CultureInfo" />。如果传递 null，则采用当前区域性。</param>
        /// <param name="value">要转换的 <see cref="T:System.Object" />。</param>
        /// <param name="destinationType">
        /// <paramref name="value" /> 参数要转换成的 <see cref="T:System.Type" />。</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destinationType" /> 参数为 null。</exception>
        /// <exception cref="T:System.NotSupportedException">不能执行转换。</exception>
        /// <exception cref="InvalidCastException"> 转换失败 </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            if (_convertor.OutputType.IsAssignableFrom(destinationType) == false)
            {
               throw new NotSupportedException("不能执行转换。");
            }
            return ConvertFrom(context, culture, value);
        }

        /// <summary>在已知对象的属性值集的情况下，使用指定的上下文创建与此 <see cref="T:System.ComponentModel.TypeConverter" /> 关联的类型的实例。</summary>
        /// <returns>一个 <see cref="T:System.Object" />，表示给定的 <see cref="T:System.Collections.IDictionary" />，或者如果无法创建该对象，则为 null。此方法始终返回 null。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        /// <param name="propertyValues">新属性值的 <see cref="T:System.Collections.IDictionary" />。</param>
        /// <exception cref="InvalidCastException"> 转换失败 </exception>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) 
            => ConvertFrom(context, null, propertyValues);

        /// <summary>
        /// 返回有关更改该对象上的某个值是否需要使用指定的上下文调用 <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)" /> 以创建新值的情况。
        /// </summary>
        /// <returns>如果更改此对象的属性需要调用 <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)" /> 来创建新值，则为 true；否则为 false。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => false;

        /// <summary>
        /// 使用指定的上下文和特性返回由 value 参数指定的数组类型的属性的集合。
        /// </summary>
        /// <returns>具有为此数据类型公开的属性的 <see cref="T:System.ComponentModel.PropertyDescriptorCollection" />；或者如果没有属性，则为 null。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        /// <param name="value">一个 <see cref="T:System.Object" />，指定要为其获取属性的数组类型。</param>
        /// <param name="attributes">用作筛选器的 <see cref="T:System.Attribute" /> 类型数组。</param>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            => null;

        /// <summary>使用指定的上下文返回该对象是否支持属性。</summary>
        /// <returns>如果应调用 <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)" /> 来查找此对象的属性，则为 true；否则为 false。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) => false;

        /// <summary>当与格式上下文一起提供时，返回此类型转换器设计用于的数据类型的标准值集合。</summary>
        /// <returns>包含标准有效值集的 <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" />；如果数据类型不支持标准值集，则为 null。</returns>
        /// <param name="context">提供格式上下文的 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，可用来提取有关从中调用此转换器的环境的附加信息。此参数或其属性可以为 null。</param>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return base.GetStandardValues(context);
        }

        /// <summary>使用指定的上下文返回从 <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> 返回的标准值的集合是否为可能值的独占列表。</summary>
        /// <returns>如果从 <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> 返回的 <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" /> 是可能值的穷举列表，则为 true；如果还可能有其他值，则为 false。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return base.GetStandardValuesExclusive(context);
        }

        /// <summary>使用指定的上下文返回此对象是否支持可以从列表中选取的标准值集。</summary>
        /// <returns>如果应调用 <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> 来查找对象支持的一组公共值，则为 true；否则，为 false。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return base.GetStandardValuesSupported(context);
        }

        /// <summary>返回给定值对象对于此类型和此指定的上下文是否有效。</summary>
        /// <returns>如果指定值对于该对象有效，则为 true；否则为 false。</returns>
        /// <param name="context">一个 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />，提供格式上下文。</param>
        /// <param name="value">要测试其有效性的 <see cref="T:System.Object" />。</param>
        public override bool IsValid(ITypeDescriptorContext context, object value) => true;

    }
}
