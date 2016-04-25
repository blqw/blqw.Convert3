using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CSByte : SystemTypeConvertor<sbyte>
    {
        protected override sbyte ChangeType(string input, Type outputType, out bool success)
        {
            sbyte result;
            if (sbyte.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = sbyte.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(sbyte);
        }

        protected override sbyte ChangeType(object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (sbyte)1 : (sbyte)0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(sbyte);
                    case TypeCode.Byte:
                        {
                            var a = conv.ToByte(null);
                            if (a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    case TypeCode.Char:
                        {
                            var a = conv.ToChar(null);
                            if (a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < sbyte.MinValue || a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < sbyte.MinValue || a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                            
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < sbyte.MinValue || a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < sbyte.MinValue || a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                            
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < sbyte.MinValue || a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < sbyte.MinValue || a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                            
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 127)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < sbyte.MinValue || a > sbyte.MaxValue)
                            {
                                success = false;
                                return default(sbyte);
                            }
                            return (sbyte)a;
                        }
                    default:
                        break;
                }
            }
            success = false;
            return default(sbyte);
        }
    }
}
