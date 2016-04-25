using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CChar : SystemTypeConvertor<char>
    {
        protected override char ChangeType(string input, Type outputType, out bool success)
        {
            if (input.Length == 1)
            {
                success = true;
                return input[0];
            }
            success = false;
            return default(char);
        }

        protected override char ChangeType(object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? 'T' : 'F';
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(char);
                    case TypeCode.Byte:
                        return (char)conv.ToByte(null);
                    case TypeCode.Char:
                        return conv.ToChar(null);
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 255)
                            {
                                success = false;
                                return default(char);
                            }
                            return (char)a;
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
                        return (char)bs[0];
                    }
                    if (bs.Length == 2)
                    {
                        success = true;
                        return BitConverter.ToChar(bs, 0);
                    }
                }
            }
            success = false;
            return default(char);
        }

    }
}
