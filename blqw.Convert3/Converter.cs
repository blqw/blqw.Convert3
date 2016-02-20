using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class Converter : TypeConverter
    {
        private bool _ThrowError;
        private Type _SourceType;

        public Converter(Type sourceType)
        {
            _SourceType = sourceType;
            _ThrowError = true;
        }

        public Converter(Type sourceType, bool throwError) 
        {
            _ThrowError = throwError;
            _SourceType = sourceType;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (_ThrowError)
            {
                return Convert3.ChangeType(value, _SourceType);
            }
            object result;
            if (Convert3.TryChangedType(value, _SourceType, out result))
            {
                return result;
            }
            if (_SourceType.IsValueType)
            {
                return Activator.CreateInstance(_SourceType);
            }
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (_ThrowError)
            {
                return Convert3.ChangeType(value, destinationType);
            }
            object result;
            if (Convert3.TryChangedType(value, destinationType, out result))
            {
                return result;
            }
            if (destinationType.IsValueType)
            {
                return Activator.CreateInstance(destinationType);
            }
            return null;
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return true;
        }
    }
}
