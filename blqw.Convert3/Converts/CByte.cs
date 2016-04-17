using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CByte : SystemTypeConvertor<byte>
    {
        protected override bool Try(object input, out byte result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (byte)1 : (byte)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte:
                        result = conv.ToByte(null);
                        return true;
                    case TypeCode.Char:
                        result = (byte)conv.ToChar(null);
                        return true;
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
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
                    if (bs.Length == 1)
                    {
                        result = bs[0];
                        return true;
                    }
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out byte result)
        {
            if (byte.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return byte.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
