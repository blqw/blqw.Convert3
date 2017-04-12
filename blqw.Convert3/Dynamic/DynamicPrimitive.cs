using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace blqw.Dynamic
{
    /// <summary>
    /// 基于系统原始类型的动态类型
    /// </summary>
    [DebuggerDisplay("{" + nameof(_debug) + "}")]
    public class DynamicPrimitive : DynamicObject, IEquatable<object>, IComparable,
        IComparable<object>, IObjectHandle, IObjectReference
    {

        private object _debug => _value ?? "<null>";
        /// <summary>
        /// 表示一个null的动态类型
        /// </summary>
        public static readonly DynamicPrimitive Null = new DynamicPrimitive(null);

        /// <summary>
        /// 表示一个空的字符串数组
        /// </summary>
        private static readonly string[] _EmptyStrings = new string[0];

        /// <summary>
        /// 被包装的原始值
        /// </summary>
        private readonly object _value;

        /// <summary>
        /// 使用指定对象初始化实例
        /// </summary>
        /// <param name="value"> </param>
        public DynamicPrimitive(object value)
        {
            _value = value;
        }

        /// <summary>
        /// 将当前实例与同一类型的另一个对象进行比较，并返回一个整数，该整数指示当前实例在排序顺序中的位置是位于另一个对象之前、之后还是与其位置相同。
        /// </summary>
        /// <returns>
        /// 一个值，指示要比较的对象的相对顺序。返回值的含义如下：值含义小于零此实例在排序顺序中位于 <paramref name="obj" /> 之前。零此实例在排序顺序中的位置与
        /// <paramref name="obj" /> 相同。大于零此实例在排序顺序中位于 <paramref name="obj" /> 之后。
        /// </returns>
        /// <param name="obj"> 与此实例进行比较的对象。 </param>
        public int CompareTo(object obj) => Compare(this, obj);

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <returns> 如果指定的对象等于当前对象，则为 true，否则为 false。 </returns>
        /// <param name="obj"> 要与当前对象进行比较的对象。 </param>
        /// <filterpriority> 2 </filterpriority>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return _value == null;
            }
            if (ReferenceEquals(this, obj)) //相同实例
            {
                return true;
            }
            obj = (obj as IObjectReference)?.GetRealObject(new StreamingContext()) ?? obj; //获取被包装的类型
            if (Equals(_value, obj))
            {
                return true;
            }
            bool b;
            var value = _value.ChangeType(obj.GetType(), out b); //尝试类型转换后比较
            return b && Equals(value, obj);
        }

        /// <summary>
        /// 打开该对象。
        /// </summary>
        /// <returns> 已打开的对象。 </returns>
        public object Unwrap() => _value;

        /// <summary>
        /// 返回应进行反序列化的真实对象（而不是序列化流指定的对象）。
        /// </summary>
        /// <returns> 返回放入图形中的实际对象。 </returns>
        /// <param name="context"> 当前对象从其中进行反序列化的 <see cref="T:System.Runtime.Serialization.StreamingContext" />。 </param>
        public object GetRealObject(StreamingContext context) => _value;

        /// <summary>
        /// 返回所有动态成员名称的枚举。
        /// </summary>
        /// <returns> 一个包含动态成员名称的序列。 </returns>
        public override IEnumerable<string> GetDynamicMemberNames() => _EmptyStrings;

        /// <summary>
        /// 尝试转换类型
        /// </summary>
        public override bool TryConvert(ConvertBinder binder, out object result)
            => Convert3.TryChangedType(_value, binder.ReturnType, out result);

        /// <summary>
        /// 尝试获取类型的成员对象
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var p = _value?.GetType().GetProperty(binder.Name, flags);
            if ((p == null) || (p.GetIndexParameters().Length > 0))
            {
                result = Null;
                return true;
            }
            result = p.GetValue(_value).ToDynamic();
            return true;
        }

        /// <summary>
        /// 尝试调用成员的方法
        /// </summary>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (_value == null)
            {
                result = Null;
                return false;
            }
            try
            {
                result = _value.GetType().InvokeMember(
                    binder.Name,
                    BindingFlags.InvokeMethod,
                    null, _value, args).ToDynamic();
                return true;
            }
            catch
            {
                result = Null;
                return false;
            }
        }

        /// <summary>
        /// 二元操作
        /// </summary>
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            var a = _value.To<bool>();
            switch (binder.Operation)
            {
                case ExpressionType.AndAlso:
                    result = a && ((arg as DynamicPrimitive)?._value ?? arg).To<bool>();
                    return true;
                case ExpressionType.OrElse:
                    result = a || ((arg as DynamicPrimitive)?._value ?? arg).To<bool>();
                    return true;
                case ExpressionType.And:
                    result = a & ((arg as DynamicPrimitive)?._value ?? arg).To<bool>();
                    return true;
                case ExpressionType.Or:
                    result = a | ((arg as DynamicPrimitive)?._value ?? arg).To<bool>();
                    return true;
                default:
                    break;
            }
            return base.TryBinaryOperation(binder, arg, out result);
        }

        /// <summary>
        /// 一元操作
        /// </summary>
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            bool value;
            if (_value.To<bool>(out value) == false) //转型失败
            {
                result = null;
                return false;
            }
            switch (binder.Operation)
            {
                case ExpressionType.IsFalse:
                case ExpressionType.Not:
                    result = value == false;
                    return true;
                case ExpressionType.IsTrue:
                    result = value;
                    return true;
                default:
                    break;
            }
            return base.TryUnaryOperation(binder, out result);
        }

        /// <summary>
        /// 当前对象与string的隐式转换
        /// </summary>
        /// <param name="value"> </param>
        /// <returns> </returns>
        public static implicit operator string(DynamicPrimitive value) => value._value.To<string>();

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Null;
            return true;
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns> 表示当前对象的字符串。 </returns>
        /// <filterpriority> 2 </filterpriority>
        public override string ToString() => _value.To<string>() ?? "<null>";

        /// <summary>
        /// 作为默认哈希函数。
        /// </summary>
        /// <returns> 当前对象的哈希代码。 </returns>
        /// <filterpriority> 2 </filterpriority>
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        private static int Compare(DynamicPrimitive a, object b)
        {
            var b1 = b as DynamicPrimitive;
            if (ReferenceEquals(b1, null) == false)
            {
                b = b1._value;
            }
            if (ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null) ? 0 : 1;
            }
            if (a._value == null)
            {
                return -1;
            }

            var comparer = Comparer.DefaultInvariant;
            return comparer.Compare(a._value.ChangeType(b.GetType()), b);
        }

        #region 运算符重载
#pragma warning disable 1591
        // ReSharper disable MissingXmlDoc
        public static bool operator >(DynamicPrimitive a, object b) => Compare(a, b) > 0;

        public static bool operator <(DynamicPrimitive a, object b) => Compare(a, b) < 0;

        public static bool operator ==(DynamicPrimitive a, object b) => a?.Equals(b) ?? ReferenceEquals(null, b);

        public static bool operator !=(DynamicPrimitive a, object b) => !(a?.Equals(b) ?? ReferenceEquals(null, b));

        public static bool operator >=(DynamicPrimitive a, object b) => Compare(a, b) >= 0;

        public static bool operator <=(DynamicPrimitive a, object b) => Compare(a, b) <= 0;
#pragma warning restore 1591
        // ReSharper restore MissingXmlDoc
        #endregion
    }
}