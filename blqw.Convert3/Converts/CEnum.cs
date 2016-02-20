using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CEnum : AdvancedConvertor<Enum>
    {
        protected override bool Try(object input, Type enumType, out Enum result)
        {
            return Try(input, enumType, out result, true);
        }
        
        private bool Try(object input, Type enumType, out Enum result, bool switchType)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                    case TypeCode.Boolean:
                        result = null;
                        return false;
                    case TypeCode.Byte: result = (Enum)Enum.ToObject(enumType, conv.ToByte(null)); return true;
                    case TypeCode.Char: result = (Enum)Enum.ToObject(enumType, conv.ToChar(null)); return true;
                    case TypeCode.Int16: result = (Enum)Enum.ToObject(enumType, conv.ToInt16(null)); return true;
                    case TypeCode.Int32: result = (Enum)Enum.ToObject(enumType, conv.ToInt32(null)); return true;
                    case TypeCode.Int64: result = (Enum)Enum.ToObject(enumType, conv.ToInt64(null)); return true;
                    case TypeCode.SByte: result = (Enum)Enum.ToObject(enumType, conv.ToInt64(null)); return true;
                    case TypeCode.Double: result = (Enum)Enum.ToObject(enumType, conv.ToDouble(null)); return true;
                    case TypeCode.Single: result = (Enum)Enum.ToObject(enumType, conv.ToSingle(null)); return true;
                    case TypeCode.UInt16: result = (Enum)Enum.ToObject(enumType, conv.ToUInt16(null)); return true;
                    case TypeCode.UInt32: result = (Enum)Enum.ToObject(enumType, conv.ToUInt32(null)); return true;
                    case TypeCode.UInt64: result = (Enum)Enum.ToObject(enumType, conv.ToUInt64(null)); return true;
                    default:
                        break;
                }
            }
            else if (switchType)
            {
                var row = input as DataRow;
                if (row != null)
                {
                    var arr = row.ItemArray;
                    if (arr.Length > 0)
                    {
                        return Try(arr[0], enumType, out result, false);
                    }
                    result = null;
                    return false;
                }
                var rv = input as DataRowView;
                if (rv != null)
                {
                    if (rv.DataView.Table.Columns.Count > 0)
                    {
                        return Try(rv[0], enumType, out result, false);
                    }
                    result = null;
                    return false;
                }
                var reader = input as IDataReader;
                if (reader != null)
                {
                    if (reader.FieldCount > 0)
                    {
                        return Try(reader.GetValue(0), enumType, out result, false);
                    }
                    result = null;
                    return false;
                }

            }
            result = null;
            return false;
        }

        protected override bool Try(string input, Type enumType, out Enum result)
        {
            if (input == null || input.Length == 0)
            {
                ErrorContext.CastFail(input, enumType);
                result = null;
                return false;
            }
            try
            {
                result = (Enum)Enum.Parse(enumType, input, true);
            }
            catch (Exception ex)
            {
                ErrorContext.Error = ex;
                result = null;
                return false;
            }

            if (Enum.IsDefined(enumType, result))
            {
                return true;
            }
            if (Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
            {
                var b = result.ToString().Contains(", ");
                if (b)
                {
                    return true;
                }
                ErrorContext.Error = new InvalidCastException(result.ToString() + " 不是有效的值");
                return false;
            }
            ErrorContext.CastFail(input, enumType);
            return false;

        }
    }
}
