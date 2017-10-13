using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;

namespace blqw
{
    public partial class Convert3
    {
        /// <summary>
        /// 将 <seealso cref="Type" /> 对象转为对应 <seealso cref="DbType" /> 对象
        /// </summary>
        public static DbType TypeToDbType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    return DbType.Boolean;
                case TypeCode.DBNull:
                    return DbType.Object;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Empty:
                    return DbType.Object;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                case TypeCode.Object:
                default:
                    break;
            }
            if (type == typeof(Guid))
            {
                return DbType.Guid;
            }
            if (type == typeof(byte[]))
            {
                return DbType.Binary;
            }
            if (type == typeof(XmlDocument))
            {
                return DbType.Xml;
            }
            return DbType.Object;
        }

        /// <summary>
        /// 将 <seealso cref="DbType" /> 对象转为对应 <seealso cref="Type" /> 对象
        /// </summary>
        public static Type DbTypeToType(DbType dbtype)
        {
            switch (dbtype)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return typeof(string);
                case DbType.Binary:
                    return typeof(byte[]);
                case DbType.Boolean:
                    return typeof(bool);
                case DbType.Byte:
                    return typeof(byte);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return typeof(DateTime);
                case DbType.Decimal:
                case DbType.VarNumeric:
                case DbType.Currency:
                    return typeof(decimal);
                case DbType.Double:
                    return typeof(double);
                case DbType.Guid:
                    return typeof(Guid);
                case DbType.Int16:
                    return typeof(short);
                case DbType.Int32:
                    return typeof(int);
                case DbType.Int64:
                    return typeof(long);
                case DbType.Object:
                    return typeof(object);
                case DbType.SByte:
                    return typeof(sbyte);
                case DbType.Single:
                    return typeof(float);
                case DbType.UInt16:
                    return typeof(ushort);
                case DbType.UInt32:
                    return typeof(uint);
                case DbType.UInt64:
                    return typeof(ulong);
                case DbType.Xml:
                    return typeof(XmlDocument);
                default:
                    throw new InvalidCastException("无效的DbType值:" + dbtype);
            }
        }

    }
}
