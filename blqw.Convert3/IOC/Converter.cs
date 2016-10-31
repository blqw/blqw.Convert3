using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using blqw.Converts;

namespace blqw.IOC
{
    /// <summary>
    /// 对象转换器
    /// </summary>
    [Export("Convert3", typeof(IFormatterConverter))]
    [Export(typeof(IFormatterConverter))]
    [ExportMetadata("Priority", ExportComponent.PRIORITY)]
    internal class ObjectConverter : IFormatterConverter
    {
        public virtual object Convert(object input, Type type)
        {
            if (type == null)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return input.ChangeType(type);
        }

        public virtual T Convert<T>(object input) => input.To<T>();
        
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
                    return value.To<bool>();
                case TypeCode.Char:
                    return value.To<char>();
                case TypeCode.SByte:
                    return value.To<sbyte>();
                case TypeCode.Byte:
                    return value.To<byte>();
                case TypeCode.Int16:
                    return value.To<short>();
                case TypeCode.UInt16:
                    return value.To<ushort>();
                case TypeCode.Int32:
                    return value.To<int>();
                case TypeCode.UInt32:
                    return value.To<uint>();
                case TypeCode.Int64:
                    return value.To<long>();
                case TypeCode.UInt64:
                    return value.To<ulong>();
                case TypeCode.Single:
                    return value.To<float>();
                case TypeCode.Double:
                    return value.To<double>();
                case TypeCode.Decimal:
                    return value.To<decimal>();
                case TypeCode.DateTime:
                    return value.To<DateTime>();
                case TypeCode.String:
                    return value.To<string>();
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
            return value.To<bool>();
        }

        byte IFormatterConverter.ToByte(object value)
        {
            return value.To<byte>();
        }

        char IFormatterConverter.ToChar(object value)
        {
            return value.To<char>();
        }

        DateTime IFormatterConverter.ToDateTime(object value)
        {
            return value.To<DateTime>();
        }

        decimal IFormatterConverter.ToDecimal(object value)
        {
            return value.To<decimal>();
        }

        double IFormatterConverter.ToDouble(object value)
        {
            return value.To<double>();
        }

        short IFormatterConverter.ToInt16(object value)
        {
            return value.To<short>();
        }

        int IFormatterConverter.ToInt32(object value)
        {
            return value.To<int>();
        }

        long IFormatterConverter.ToInt64(object value)
        {
            return value.To<long>();
        }

        sbyte IFormatterConverter.ToSByte(object value)
        {
            return value.To<sbyte>();
        }

        float IFormatterConverter.ToSingle(object value)
        {
            return value.To<float>();
        }

        string IFormatterConverter.ToString(object value)
        {
            return value.To<string>();
        }

        ushort IFormatterConverter.ToUInt16(object value)
        {
            return value.To<ushort>();
        }

        uint IFormatterConverter.ToUInt32(object value)
        {
            return value.To<uint>();
        }

        ulong IFormatterConverter.ToUInt64(object value)
        {
            return value.To<ulong>();
        }

        #endregion
    }
}