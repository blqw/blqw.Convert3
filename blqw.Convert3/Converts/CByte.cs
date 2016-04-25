using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CByte : SystemTypeConvertor<byte>
    {
        protected override byte ChangeType(string input, Type outputType, out bool success)
        {
            byte result;
            if (byte.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = byte.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return 0;
        }

        protected override byte ChangeType(object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (byte)1 : (byte)0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(byte);
                    case TypeCode.Byte:
                        return conv.ToByte(null);

                    case TypeCode.Char:
                        return (byte)conv.ToChar(null);

                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(byte);
                            }
                            return (byte)a;
                        }
                    default:
                        break;
                }
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 1)
                    {
                        success = true;
                        return bs[0];
                    }
                }
            }
            success = false;
            return default(byte);
        }
        
    }
}
