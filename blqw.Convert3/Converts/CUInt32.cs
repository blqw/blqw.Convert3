using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CUInt32 : SystemTypeConvertor<UInt32>
    {
        protected override bool Try(object input, out uint result)
        {
            if (input == null)
            {
                result = 0;
                return false;
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (uint)1 : (uint)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = (uint)conv.ToByte(null); return true;
                    case TypeCode.Char: result = (uint)conv.ToChar(null); return true;
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
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
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = (uint)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = conv.ToUInt32(null); return true;
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input is IntPtr)
            {
                var a = ((IntPtr)input).ToInt64();
                if (a < 0 || a > 4294967295)
                {
                    result = 0;
                    return false;
                }
                result = (uint)a;
                return true;
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > 4294967295)
                {
                    result = 0;
                    return false;
                }
                result = (uint)a;
                return true;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null && bs.Length == 4)
                {
                    result = BitConverter.ToUInt32(bs, 0);
                    return true;
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out uint result)
        {
            if (uint.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return uint.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
