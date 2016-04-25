﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CInt16 : SystemTypeConvertor<short>
    {
        protected override short ChangeType(string input, Type outputType, out bool success)
        {
            short result;
            if (short.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = short.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(short);
        }

        protected override short ChangeType(object input, Type outputType, out bool success)
        {
            success = true;
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (short)1 : (short)0;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(short);
                    case TypeCode.Byte: return conv.ToByte(null);
                    case TypeCode.Char: return (short)conv.ToChar(null);
                    case TypeCode.Int16: return conv.ToInt16(null);
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < -32768 || a > 32767)
                            {
                                success = false;
                                return default(short);
                            }
                            return (short)a;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < -32768 || a > 32767)
                            {
                                success = false;
                                return default(short);
                            }
                            return (short)a;
                        }
                    case TypeCode.SByte: return (short)conv.ToSByte(null);
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -32768 || a > 32767)
                            {
                                success = false;
                                return default(short);
                            }
                            return (short)a;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -32768 || a > 32767)
                            {
                                success = false;
                                return default(short);
                            }
                            return (short)a;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 32767)
                            {
                                success = false;
                                return default(short);
                            }
                            return (short)a;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 32767)
                            {
                                success = false;
                                return default(short);
                            }
                            return (short)a;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 32767)
                            {
                                success = false;
                                return default(short);
                            }
                            return (short)a;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -32768 || a > 32767)
                            {
                                success = false;
                                return default(short);
                            }
                            return (short)a;
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
                    if (bs.Length == 2)
                    {
                        return BitConverter.ToInt16(bs, 0);
                    }
                }
            }
            success = false;
            return default(short);
        }
        
    }
}
