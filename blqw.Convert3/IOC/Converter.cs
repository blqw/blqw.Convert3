using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using blqw;
using blqw.Converts;

namespace blqw.IOC
{
    /// <summary>
    /// 对象转换器
    /// </summary>
    [Export("Convert3", typeof(IFormatterConverter))]
    [Export(typeof(IFormatterConverter))]
    [ExportMetadata("Priority", IOC.ExportComponent.PRIORITY)]
    class ObjectConverter : IFormatterConverter
    {
        public virtual object Convert(object input, Type type)
        {
            if (type == null)
            {
                throw new ArgumentOutOfRangeException("type");
            }
            return input.ChangeType(type);
        }

        public virtual T Convert<T>(object input)
        {
            return input.To<T>();
        }

        #region IFormatterConverter 成员

        object IFormatterConverter.Convert(object value, TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Empty:
                    return null;
                case TypeCode.Object:
                    return value;
                case TypeCode.DBNull:
                    return DBNull.Value;
                case TypeCode.Boolean:
                    return Convert3.To<bool>(value);
                case TypeCode.Char:
                    return Convert3.To<Char>(value);
                case TypeCode.SByte:
                    return Convert3.To<SByte>(value);
                case TypeCode.Byte:
                    return Convert3.To<Byte>(value);
                case TypeCode.Int16:
                    return Convert3.To<Int16>(value);
                case TypeCode.UInt16:
                    return Convert3.To<UInt16>(value);
                case TypeCode.Int32:
                    return Convert3.To<Int32>(value);
                case TypeCode.UInt32:
                    return Convert3.To<UInt32>(value);
                case TypeCode.Int64:
                    return Convert3.To<Int64>(value);
                case TypeCode.UInt64:
                    return Convert3.To<UInt64>(value);
                case TypeCode.Single:
                    return Convert3.To<Single>(value);
                case TypeCode.Double:
                    return Convert3.To<Double>(value);
                case TypeCode.Decimal:
                    return Convert3.To<Decimal>(value);
                case TypeCode.DateTime:
                    return Convert3.To<DateTime>(value);
                case TypeCode.String:
                    return Convert3.To<String>(value);
                default:
                    break;
            }
            var type = CType.GetType(typeCode);
            if (type == null)
            {
                throw new ArgumentOutOfRangeException("typeCode");
            }
            return Convert(value, type);
        }

        bool IFormatterConverter.ToBoolean(object value)
        {
            return Convert3.To<bool>(value);
        }

        byte IFormatterConverter.ToByte(object value)
        {
            return Convert3.To<byte>(value);
        }

        char IFormatterConverter.ToChar(object value)
        {
            return Convert3.To<char>(value);
        }

        DateTime IFormatterConverter.ToDateTime(object value)
        {
            return Convert3.To<DateTime>(value);
        }

        decimal IFormatterConverter.ToDecimal(object value)
        {
            return Convert3.To<decimal>(value);
        }

        double IFormatterConverter.ToDouble(object value)
        {
            return Convert3.To<double>(value);
        }

        short IFormatterConverter.ToInt16(object value)
        {
            return Convert3.To<short>(value);
        }

        int IFormatterConverter.ToInt32(object value)
        {
            return Convert3.To<int>(value);
        }

        long IFormatterConverter.ToInt64(object value)
        {
            return Convert3.To<long>(value);
        }

        sbyte IFormatterConverter.ToSByte(object value)
        {
            return Convert3.To<sbyte>(value);
        }

        float IFormatterConverter.ToSingle(object value)
        {
            return Convert3.To<float>(value);
        }

        string IFormatterConverter.ToString(object value)
        {
            return Convert3.To<string>(value);
        }

        ushort IFormatterConverter.ToUInt16(object value)
        {
            return Convert3.To<ushort>(value);
        }

        uint IFormatterConverter.ToUInt32(object value)
        {
            return Convert3.To<uint>(value);
        }

        ulong IFormatterConverter.ToUInt64(object value)
        {
            return Convert3.To<ulong>(value);
        }

        #endregion
    }

    /// <summary>
    /// 定向转换器
    /// </summary>
    class DirectConverter : ObjectConverter
    {
        Type _OutputType;
        bool _ThrowError;
        IConvertor _Convertor;
        IConvertor Convertor
        {
            get
            {
                return _Convertor ?? (_Convertor = ConvertorServices.Container.GetConvertor(_OutputType));
            }
        }

        public DirectConverter(Type outputType, bool throwError)
        {
            _OutputType = outputType;
            _ThrowError = throwError;
        }

        public override object Convert(object input, Type type)
        {
            using (Error.Contract())
            {
                bool b;
                var output = Convertor.ChangeType(input, type, out b);
                if (_ThrowError && b == false)
                {
                    Error.ThrowIfHaveError();
                }
                return output;
            }
        }

        public override T Convert<T>(object input)
        {
            using (Error.Contract())
            {
                bool b;
                var output = Convertor.ChangeType(input, typeof(T), out b);
                if (_ThrowError && b == false)
                {
                    Error.ThrowIfHaveError();
                }
                return (T)output;
            }
        }
    }

}
