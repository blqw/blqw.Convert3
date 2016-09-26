using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CUInt64 : SystemTypeConvertor<ulong>
    {
        protected override ulong ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            ulong result;
            if (ulong.TryParse(input, out result))
            {
                success = true;
                return result;
            }
            if (CString.IsHexString(ref input))
            {
                success = ulong.TryParse(input, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result);
                return result;
            }
            success = false;
            return default(ulong);
        }

        protected override ulong ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                success = true;
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return conv.ToBoolean(null) ? (ulong)1 : (ulong)0;                        
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        success = false;
                        return default(ulong);
                    case TypeCode.Byte: return (ulong)conv.ToByte(null); 
                    case TypeCode.Char: return (ulong)conv.ToChar(null); 
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(ulong);
                            }
                            return (ulong)a;                            
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(ulong);
                            }
                            return (ulong)a;                            
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(ulong);
                            }
                            return (ulong)a;                            
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(ulong);
                            }
                            return (ulong)a;                            
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(ulong);
                            }
                            return (ulong)a;                            
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(ulong);
                            }
                            return (ulong)a;                            
                        }
                    case TypeCode.UInt16: return (ulong)conv.ToUInt16(null); 
                    case TypeCode.UInt32: return (ulong)conv.ToUInt32(null); 
                    case TypeCode.UInt64: return conv.ToUInt64(null); 
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0)
                            {
                                success = false;
                                return default(ulong);
                            }
                            return (ulong)a;                            
                        }
                    default:
                        break;
                }
            }
            else if (input is IntPtr)
            {
                var a = ((IntPtr)input).ToInt64();
                if (a < 0)
                {
                    success = false;
                    return default(ulong);
                }
                success = true;
                return (ulong)a;
                
            }
            else if (input is UIntPtr)
            {
                success = true;
                return ((UIntPtr)input).ToUInt64();                
            }
            else
            {
                var bs = input as byte[];
                if (bs != null && bs.Length == 8)
                {
                    success = true;
                    return BitConverter.ToUInt64(bs, 0);                    
                }
            }
            success = false;
            return default(ulong);
        }
        
    }
}
