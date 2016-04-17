using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CSByte : SystemTypeConvertor<SByte>
    {
        [System.ComponentModel.Composition.Export(typeof(IConvertor))]
        protected override bool Try(object input, out sbyte result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (sbyte)1 : (sbyte)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte:
                        {
                            var a = conv.ToByte(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Char:
                        {
                            var a = conv.ToChar(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            result = 0;
            return false;
        }

        protected override bool Try(string input, out sbyte result)
        {
            if (sbyte.TryParse(input, out result))
            {
                return true;
            }
            if (CString.IsHexString(ref input))
            {
                return sbyte.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
            }
            result = 0;
            return false;
        }
    }
}
