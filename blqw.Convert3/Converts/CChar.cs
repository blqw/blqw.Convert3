using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CChar : SystemTypeConvertor<char>
    {
        protected override bool Try(object input, out char result)
        {
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? 'T' : 'F';
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = default(char);
                        return false;
                    case TypeCode.Byte:
                        result = (char)conv.ToByte(null);
                        return true;
                    case TypeCode.Char:
                        result = conv.ToChar(null);
                        return true;
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
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
                        result = (char)bs[0];
                        return true;
                    }
                    if (bs.Length == 2)
                    {
                        result = BitConverter.ToChar(bs, 0);
                        return true;
                    }
                }
            }
            result = default(char);
            return false;
        }

        protected override bool Try(string input, out char result)
        {
            if (input != null && input.Length == 1)
            {
                result = input[0];
                return true;
            }
            result = default(char);
            return false;
        }
    }
}
