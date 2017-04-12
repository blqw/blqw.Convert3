using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace blqw.Dynamic
{
    /// <summary>
    /// 基于 <seealso cref="object"/> 的动态类型
    /// </summary>
    [DebuggerDisplay("{" + nameof(_entity) + "}")]
    public class DynamicEntity : DynamicObject, IObjectHandle, IObjectReference, ICustomTypeProvider
    {
        private readonly object _entity;
        private readonly PropertyHandler[] _properties;
        private readonly int _propertyCount;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="entity"></param>
        public DynamicEntity(object entity)
        {
            if (entity == null)
            {
                return;
            }
            _entity = entity;
            _properties = PublicPropertyCache.GetByType(entity.GetType());
            _propertyCount = _properties.Length;
        }

        /// <summary>
        /// 根据名称获取属性,不区分大小写
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private PropertyHandler this[string name]
        {
            get
            {
                for (var i = 0; i < _propertyCount; i++)
                {
                    var p = _properties[i];
                    if (string.Equals(name, p.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return p;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 获取由此对象提供的自定义类型。
        /// </summary>
        /// <returns> 自定义类型。 </returns>
        public Type GetCustomType() => _entity?.GetType() ?? typeof(object);


        /// <summary>
        /// 打开该对象。
        /// </summary>
        /// <returns> 已打开的对象。 </returns>
        public object Unwrap() => _entity;

        /// <summary>
        /// 返回应进行反序列化的真实对象（而不是序列化流指定的对象）。
        /// </summary>
        /// <returns> 返回放入图形中的实际对象。 </returns>
        /// <param name="context"> 当前对象从其中进行反序列化的 <see cref="T:System.Runtime.Serialization.StreamingContext" />。 </param>
        public object GetRealObject(StreamingContext context) => _entity;

        /// <summary>
        /// 返回所有动态成员名称的枚举。</summary>
        /// <returns>一个包含动态成员名称的序列。</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            for (var i = 0; i < _propertyCount; i++)
            {
                yield return _properties[i].Name;
            }
        }

        /// <summary>
        /// 提供类型转换运算的实现。 从 <see cref="T:System.Dynamic.DynamicObject" /> 类派生的类可以重写此方法，以便为将某个对象从一种类型转换为另一种类型的运算指定动态行为。</summary>
        /// <returns>如果此运算成功，则为 true；否则为 false。 如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）</returns>
        /// <param name="binder">提供有关转换运算的信息。 binder.Type 属性提供必须将对象转换为的类型。 例如，对于 C# 中的语句 (String)sampleObject（Visual Basic 中为 CType(sampleObject, Type)）（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject" /> 类的类的一个实例），binder.Type 将返回 <see cref="T:System.String" /> 类型。 binder.Explicit 属性提供有关所发生转换的类型的信息。 对于显式转换，它返回 true；对于隐式转换，它返回 false。</param>
        /// <param name="result">类型转换运算的结果。</param>
        public override bool TryConvert(ConvertBinder binder, out object result) => Convert3.TryChangedType(_entity, binder.ReturnType, out result);

        /// <summary>
        /// 为获取成员值的操作提供实现。 从 <see cref="T:System.Dynamic.DynamicObject" /> 类派生的类可以重写此方法，以便为诸如获取属性值这样的操作指定动态行为。</summary>
        /// <returns>如果此运算成功，则为 true；否则为 false。 如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。）</returns>
        /// <param name="binder">提供有关调用了动态操作的对象的信息。 binder.Name 属性提供针对其执行动态操作的成员的名称。 例如，对于 Console.WriteLine(sampleObject.SampleProperty) 语句（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject" /> 类的类的一个实例），binder.Name 将返回“SampleProperty”。 binder.IgnoreCase 属性指定成员名称是否区分大小写。</param>
        /// <param name="result">获取操作的结果。 例如，如果为某个属性调用该方法，则可以为 <paramref name="result" /> 指派该属性值。</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var p = this[binder.Name];
            if (p?.Get != null)
            {
                result = p.Get(_entity);
                bool b;
                var r = result.ChangeType(binder.ReturnType, out b);
                result = b ? r.ToDynamic() : DynamicPrimitive.Null;
                return b;
            }
            result = DynamicPrimitive.Null;
            return true;
        }

        /// <summary>
        /// 为设置成员值的操作提供实现。 从 <see cref="T:System.Dynamic.DynamicObject" /> 类派生的类可以重写此方法，以便为诸如设置属性值这样的操作指定动态行为。</summary>
        /// <returns>如果此运算成功，则为 true；否则为 false。 如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）</returns>
        /// <param name="binder">提供有关调用了动态操作的对象的信息。 binder.Name 属性提供将该值分配到的成员的名称。 例如，对于语句 sampleObject.SampleProperty = "Test"（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject" /> 类的类的一个实例），binder.Name 将返回“SampleProperty”。 binder.IgnoreCase 属性指定成员名称是否区分大小写。</param>
        /// <param name="value">要为成员设置的值。 例如，对于 sampleObject.SampleProperty = "Test"（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject" /> 类的类的一个实例），<paramref name="value" /> 为“Test”。</param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var p = this[binder.Name];
            if (p == null)
            {
                return false;
            }
            return p.SetValue(ConvertContext.None, _entity, value);
        }
    }
}