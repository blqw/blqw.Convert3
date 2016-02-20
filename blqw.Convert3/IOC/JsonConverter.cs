using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using System.Runtime.Serialization;

namespace blqw.Convert3Component
{
    [Export("JsonConverter", typeof(IFormatterConverter))]
    class JsonConverter : IFormatterConverter
    {
        [Export("ToJsonString")]
        public static string ToJsonString(object obj)
        {
            return JSON.Serialize(obj);
        }

        //导出 ToJsonObject 插件
        [Export("ToJsonObject")]
        public static object ToJsonObject(Type type, string json)
        {
            return JSON.Deserialize(json, type);
        }
        static readonly JavaScriptSerializer JSON = new JavaScriptSerializer();

        public object Convert(object value, TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Empty:
                    return null;
                case TypeCode.Object:
                    return Convert(value, typeof(object));
                case TypeCode.DBNull:
                    return DBNull.Value;
                case TypeCode.Boolean:
                    return Convert(value, typeof(Boolean));
                case TypeCode.Char:
                    return Convert(value, typeof(Char));
                case TypeCode.SByte:
                    return Convert(value, typeof(SByte));
                case TypeCode.Byte:
                    return Convert(value, typeof(Byte));
                case TypeCode.Int16:
                    return Convert(value, typeof(Int16));
                case TypeCode.UInt16:
                    return Convert(value, typeof(UInt16));
                case TypeCode.Int32:
                    return Convert(value, typeof(Int32));
                case TypeCode.UInt32:
                    return Convert(value, typeof(UInt32));
                case TypeCode.Int64:
                    return Convert(value, typeof(Int64));
                case TypeCode.UInt64:
                    return Convert(value, typeof(UInt64));
                case TypeCode.Single:
                    return Convert(value, typeof(Single));
                case TypeCode.Double:
                    return Convert(value, typeof(Double));
                case TypeCode.Decimal:
                    return Convert(value, typeof(Decimal));
                case TypeCode.DateTime:
                    return Convert(value, typeof(DateTime));
                case TypeCode.String:
                    return Convert(value, typeof(String));
                default:
                    throw new InvalidCastException("类型未知:TypeCode=" + typeCode);
            }
        }

        public object Convert(object value, Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type == typeof(string))
            {
                return JSON.Serialize(value);
            }
            var json = value as string;
            if (json == null)
            {
                if (value == null)
                {
                    if (type.IsValueType)
                    {
                        return Activator.CreateInstance(type);
                    }
                    return null;
                }
                throw new InvalidCastException("仅支持`对象`与`Json字符串`之间的转换");
            }
            return JSON.Deserialize(json, type);
        }

        public bool ToBoolean(object value)
        {
            return (bool)Convert(value, typeof(bool));
        }

        public byte ToByte(object value)
        {
            return (byte)Convert(value, typeof(byte));
        }

        public char ToChar(object value)
        {
            return (char)Convert(value, typeof(char));
        }

        public DateTime ToDateTime(object value)
        {
            return (DateTime)Convert(value, typeof(DateTime));
        }

        public decimal ToDecimal(object value)
        {
            return (decimal)Convert(value, typeof(decimal));
        }

        public double ToDouble(object value)
        {
            return (double)Convert(value, typeof(double));
        }

        public short ToInt16(object value)
        {
            return (short)Convert(value, typeof(short));
        }

        public int ToInt32(object value)
        {
            return (int)Convert(value, typeof(int));
        }

        public long ToInt64(object value)
        {
            return (long)Convert(value, typeof(long));
        }

        public sbyte ToSByte(object value)
        {
            return (sbyte)Convert(value, typeof(sbyte));
        }

        public float ToSingle(object value)
        {
            return (float)Convert(value, typeof(float));
        }

        public string ToString(object value)
        {
            return JSON.Serialize(value);
        }

        public ushort ToUInt16(object value)
        {
            return (ushort)Convert(value, typeof(ushort));
        }

        public uint ToUInt32(object value)
        {
            return (uint)Convert(value, typeof(uint));
        }

        public ulong ToUInt64(object value)
        {
            return (ulong)Convert(value, typeof(ulong));
        }

    }

}
