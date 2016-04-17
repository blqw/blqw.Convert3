using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CInt16 : SystemTypeConvertor<Int16>
    {
        protected override bool Try(object input, out short result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (short)1 : (short)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null); return true;
                    case TypeCode.Char: result = (short)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = conv.ToInt16(null); return true;
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.SByte: result = (short)conv.ToSByte(null); return true;
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
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
                        result = BitConverter.ToInt16(bs, 0);
                        return true;
                    }
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out short result)
        {
            if (short.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return short.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
