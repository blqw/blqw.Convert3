using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Dynamic
{
    public class DynamicSystemObject : DynamicObject, IEquatable<object>, IEqualityComparer<object>, IComparable, IComparable<object>, IFormatProvider
    {
        public static readonly DynamicSystemObject Null = new DynamicSystemObject(null);

        object _value;
        public DynamicSystemObject(object value)
        {
            _value = value;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return new string[0];
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return Convert3.TryChangedType(_value, binder.ReturnType, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Null;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (_value == null)
            {
                result = null;
                return false;
            }
            try
            {
                result = _value.GetType().InvokeMember(
                             binder.Name,
                             System.Reflection.BindingFlags.InvokeMethod,
                             null, _value, args);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.And:
                case ExpressionType.Or:
                    var x = arg as DynamicSystemObject;
                    if (object.ReferenceEquals(x, null) == false)
                    {
                        bool b;
                        if (_value.TryTo<bool>(out b) == false)
                        {
                            result = null;
                            return false;
                        }
                        result = b;
                    }
                    else
                    {
                        result = arg.To<bool>();
                    }
                    return true;
                default:
                    break;
            }
            return base.TryBinaryOperation(binder, arg, out result);
        }
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            bool b;
            if (_value.TryTo<bool>(out b) == false)
            {
                result = null;
                return false;
            }
            switch (binder.Operation)
            {
                case ExpressionType.IsFalse:
                case ExpressionType.Not:
                    result = b == false;
                    return true;
                case ExpressionType.IsTrue:
                    result = b == true;
                    return true;
                default:
                    break;
            }
            return base.TryUnaryOperation(binder, out result);
        }

        public static implicit operator string(DynamicSystemObject value)
        {
            return value._value.To<string>();
        }
        
        #region 运算符重载

        public static bool operator >(DynamicSystemObject a, object b)
        {
            return Compare(a, b) > 0;
        }

        public static bool operator <(DynamicSystemObject a, object b)
        {
            return Compare(a, b) < 0;
        }

        public static bool operator ==(DynamicSystemObject a, object b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(DynamicSystemObject a, object b)
        {
            return Equals(a, b) == false;
        }


        public static bool operator >=(DynamicSystemObject a, object b)
        {
            return Compare(a, b) >= 0;
        }

        public static bool operator <=(DynamicSystemObject a, object b)
        {
            return Compare(a, b) <= 0;
        }


        #endregion



        public override string ToString()
        {
            return _value.To<string>();
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj);
        }

        public override int GetHashCode()
        {
            if (object.ReferenceEquals(_value, null))
            {
                return 0;
            }
            return _value.GetHashCode();
        }

        public bool Equals(object x, object y)
        {
            var t = y as DynamicSystemObject;
            if (object.ReferenceEquals(t, null) == false)
            {
                return Equals(t, y);
            }
            t = x as DynamicSystemObject;
            if (object.ReferenceEquals(t, null) == false)
            {
                return Equals(t, x);
            }
            return object.Equals(x, y);
        }

        public int GetHashCode(object obj)
        {
            if (object.ReferenceEquals(obj, null))
            {
                return 0;
            }
            return obj.GetHashCode();
        }

        public int CompareTo(object other)
        {
            return Compare(this, other);
        }

        static int Compare(DynamicSystemObject a, object b)
        {
            var b1 = b as DynamicSystemObject;
            if (object.ReferenceEquals(b1, null) == false)
            {
                b = b1._value;
            }
            if (object.ReferenceEquals(b, null)) return object.ReferenceEquals(a, null) ? 0 : 1;
            if (a._value == null) return -1;

            var comparer = System.Collections.Comparer.DefaultInvariant;
            return comparer.Compare(a._value.ChangeType(b.GetType()), b);
        }

        static bool Equals(DynamicSystemObject a, object b)
        {
            var t = b as DynamicSystemObject;
            if (object.ReferenceEquals(t, null) == false)
            {
                b = t._value;
            }
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(a._value, null))
            {
                return object.ReferenceEquals(b, null);
            }
            if (object.ReferenceEquals(b, null))
            {
                return false;
            }
            return a._value.ChangeType(b.GetType()).Equals(b);
        }

        object IFormatProvider.GetFormat(Type formatType)
        {
            if (formatType != null && string.Equals("Json", formatType.Name, StringComparison.Ordinal))
            {
                return _value ?? DBNull.Value;
            }
            return null;
        }
    }
}
