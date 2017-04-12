using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace blqw.Dynamic
{
    /// <summary>
    /// 基于 <seealso cref="NameValueCollection" /> 的动态类型
    /// </summary>
    [DebuggerDisplay("{" + nameof(_items) + "}")]
    public class DynamicNameValueCollection : DynamicObject, IObjectHandle, IObjectReference, ICustomTypeProvider
    {
        private readonly NameValueCollection _items;

        /// <summary>
        /// 初始化默认实例
        /// </summary>
        public DynamicNameValueCollection()
        {
            _items = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 初始化默认实例,并指示属性名的比较方式
        /// </summary>
        /// <param name="comparer"> 用于确定两个键是否相等，并为集合中的键生成哈希代码。 </param>
        public DynamicNameValueCollection(StringComparer comparer)
        {
            _items = new NameValueCollection(comparer);
        }

        /// <summary>
        /// 使用指定的<seealso cref="NameValueCollection" />来初始化实例
        /// </summary>
        /// <param name="nv"> </param>
        public DynamicNameValueCollection(NameValueCollection nv)
        {
            _items = nv;
        }

        /// <summary>
        /// 获取或设置实例是否为只读
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 获取由此对象提供的自定义类型。
        /// </summary>
        /// <returns> 自定义类型。 </returns>
        public Type GetCustomType() => typeof(NameValueCollection);

        /// <summary> 打开该对象。 </summary>
        /// <returns> 已打开的对象。 </returns>
        public object Unwrap() => _items;

        /// <summary> 返回应进行反序列化的真实对象（而不是序列化流指定的对象）。 </summary>
        /// <returns> 返回放入图形中的实际对象。 </returns>
        /// <param name="context"> 当前对象从其中进行反序列化的 <see cref="T:System.Runtime.Serialization.StreamingContext" />。 </param>
        public object GetRealObject(StreamingContext context) => _items;

        /// <summary>
        /// 返回所有动态成员名称的枚举。
        /// </summary>
        /// <returns> 一个包含动态成员名称的序列。 </returns>
        public override IEnumerable<string> GetDynamicMemberNames() => _items.AllKeys;

        /// <summary>
        /// 提供类型转换运算的实现。 从 <see cref="T:System.Dynamic.DynamicObject" /> 类派生的类可以重写此方法，以便为将某个对象从一种类型转换为另一种类型的运算指定动态行为。
        /// </summary>
        /// <returns> 如果此运算成功，则为 true；否则为 false。 如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。） </returns>
        /// <param name="binder">
        /// 提供有关转换运算的信息。 binder.Type 属性提供必须将对象转换为的类型。 例如，对于 C# 中的语句 (String)sampleObject（Visual Basic 中为
        /// CType(sampleObject, Type)）（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject" /> 类的类的一个实例），binder.Type 将返回
        /// <see cref="T:System.String" /> 类型。 binder.Explicit 属性提供有关所发生转换的类型的信息。 对于显式转换，它返回 true；对于隐式转换，它返回 false。
        /// </param>
        /// <param name="result"> 类型转换运算的结果。 </param>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (typeof(IConvertible).IsAssignableFrom(binder.ReturnType))
            {
                if ((_items.Count == 1) && Convert3.TryChangedType(_items[0], binder.ReturnType, out result))
                {
                    return true;
                }
            }
            return Convert3.TryChangedType(_items, binder.ReturnType, out result);
        }

        /// <summary>
        /// 为获取成员值的操作提供实现。 从 <see cref="T:System.Dynamic.DynamicObject" /> 类派生的类可以重写此方法，以便为诸如获取属性值这样的操作指定动态行为。
        /// </summary>
        /// <returns> 如果此运算成功，则为 true；否则为 false。 如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。） </returns>
        /// <param name="binder">
        /// 提供有关调用了动态操作的对象的信息。 binder.Name 属性提供针对其执行动态操作的成员的名称。 例如，对于
        /// Console.WriteLine(sampleObject.SampleProperty) 语句（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject" />
        /// 类的类的一个实例），binder.Name 将返回“SampleProperty”。 binder.IgnoreCase 属性指定成员名称是否区分大小写。
        /// </param>
        /// <param name="result"> 获取操作的结果。 例如，如果为某个属性调用该方法，则可以为 <paramref name="result" /> 指派该属性值。 </param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _items[binder.Name];
            if (result != null)
            {
                if (Convert3.TryChangedType(result, binder.ReturnType, out result))
                {
                    result = result.ToDynamic();
                    return true;
                }
            }

            result = DynamicPrimitive.Null;
            return true;
        }

        /// <summary>
        /// 为设置成员值的操作提供实现。 从 <see cref="T:System.Dynamic.DynamicObject" /> 类派生的类可以重写此方法，以便为诸如设置属性值这样的操作指定动态行为。
        /// </summary>
        /// <returns> 如果此运算成功，则为 true；否则为 false。 如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。） </returns>
        /// <param name="binder">
        /// 提供有关调用了动态操作的对象的信息。 binder.Name 属性提供将该值分配到的成员的名称。 例如，对于语句 sampleObject.SampleProperty = "Test"（其中
        /// sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject" /> 类的类的一个实例），binder.Name 将返回“SampleProperty”。
        /// binder.IgnoreCase 属性指定成员名称是否区分大小写。
        /// </param>
        /// <param name="value">
        /// 要为成员设置的值。 例如，对于 sampleObject.SampleProperty = "Test"（其中 sampleObject 是派生自
        /// <see cref="T:System.Dynamic.DynamicObject" /> 类的类的一个实例），<paramref name="value" /> 为“Test”。
        /// </param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (IsReadOnly)
            {
                return false;
                //throw new NotSupportedException("当前对象是只读的");
            }
            _items[binder.Name] = value.To<string>();
            return true;
        }

        private object Indexer(object[] indexes)
        {
            if ((indexes == null) || (indexes.Length != 1))
            {
                return null;
            }
            var index = indexes[0];
            var name = index as string;
            if (name != null)
            {
                return _items[name];
            }
            var i = index.To(-1);
            if ((i < 0) || (i >= _items.Count))
            {
                return null;
            }
            return _items[i];
        }

        /// <summary>
        /// 为按索引获取值的操作提供实现。 从 <see cref="T:System.Dynamic.DynamicObject" /> 类派生的类可以重写此方法，以便为索引操作指定动态行为。
        /// </summary>
        /// <returns> 如果此运算成功，则为 true；否则为 false。 如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。） </returns>
        /// <param name="binder"> 提供有关该操作的信息。 </param>
        /// <param name="indexes">
        /// 该操作中使用的索引。 例如，对于 C# 中的 sampleObject[3] 操作（Visual Basic 中为 sampleObject(3)）（其中 sampleObject 派生自
        /// DynamicObject 类），<paramref name="indexes[0]" /> 等于 3。
        /// </param>
        /// <param name="result"> 索引操作的结果。 </param>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Indexer(indexes);
            if (result != null)
            {
                if (Convert3.TryChangedType(result, binder.ReturnType, out result))
                {
                    result = result.ToDynamic();
                    return true;
                }
            }

            result = DynamicPrimitive.Null;
            return true;
        }

        /// <summary>
        /// 为按索引设置值的操作提供实现。 从 <see cref="T:System.Dynamic.DynamicObject" /> 类派生的类可以重写此方法，以便为按指定索引访问对象的操作指定动态行为。
        /// </summary>
        /// <returns> 如果此运算成功，则为 true；否则为 false。 如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。） </returns>
        /// <param name="binder"> 提供有关该操作的信息。 </param>
        /// <param name="indexes">
        /// 该操作中使用的索引。 例如，对于 C# 中的 sampleObject[3] = 10 操作（Visual Basic 中为 sampleObject(3) = 10）（其中
        /// sampleObject 派生自 <see cref="T:System.Dynamic.DynamicObject" /> 类），<paramref name="indexes[0]" /> 等于 3。
        /// </param>
        /// <param name="value">
        /// 要为具有指定索引的对象设置的值。 例如，对于 C# 中的 sampleObject[3] = 10 操作（Visual Basic 中为 sampleObject(3) = 10）（其中
        /// sampleObject 派生自 <see cref="T:System.Dynamic.DynamicObject" /> 类），<paramref name="value" /> 等于 10。
        /// </param>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (IsReadOnly)
            {
                return false;
            }

            if (indexes.Length != 1)
            {
                return false;
            }
            var index = indexes[0];
            var name = index as string;
            var str = value.To<string>();
            if (name != null)
            {
                _items[name] = str;
                return true;
            }
            var i = index.To(-1);
            if ((i < 0) || (i >= _items.Count))
            {
                return false;
            }
            _items[_items.GetKey(i)] = str;
            return true;
        }
    }
}