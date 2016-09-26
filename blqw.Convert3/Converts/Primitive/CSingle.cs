using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CSingle : SystemTypeConvertor<float>
    {
        protected override float ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            float result;
            if (float.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = float.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return 0;
        }

        protected override float ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (Single)1 : (Single)0;

                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(float);
                    case TypeCode.Byte:
                        return conv.ToByte(null);
                    case TypeCode.Char:
                        return (Single)conv.ToChar(null);
                    case TypeCode.Int16: return (Single)conv.ToInt16(null);
                    case TypeCode.Int32: return (Single)conv.ToInt32(null);
                    case TypeCode.Int64: return (Single)conv.ToInt64(null);
                    case TypeCode.SByte: return (Single)conv.ToSByte(null);
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < Single.MinValue || a > Single.MaxValue)
                            {
                                success = false;
                                return default(float);
                            }
                            return (Single)a;

                        }
                    case TypeCode.Single: return (Single)conv.ToSingle(null);
                    case TypeCode.UInt16: return (Single)conv.ToUInt16(null);
                    case TypeCode.UInt32: return (Single)conv.ToUInt32(null);
                    case TypeCode.UInt64: return (Single)conv.ToUInt64(null);
                    case TypeCode.Decimal: return (Single)conv.ToDecimal(null);
                    default:
                        break;
                }
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 4)
                    {
                        success = true;
                        return BitConverter.ToSingle(bs, 0);
                    }
                }
            }
            success = false;
            return default(float);
        }

    }
}
