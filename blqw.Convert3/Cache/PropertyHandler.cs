using System;
using System.Reflection;
using blqw.IOC;

namespace blqw
{
    /// <summary>
    /// 用于操作属性的Get和Set
    /// </summary>
    internal class PropertyHandler
    {
        /// <summary>
        /// 初始化 <see cref="PropertyHandler"/>
        /// </summary>
        /// <param name="property">属性</param>
        public PropertyHandler(PropertyInfo property)
        {
            Property = property;
            PropertyType = property.PropertyType;
            Name = property.Name;
            Get = ComponentServices.GetGeter(property);
            Set = ComponentServices.GetSeter(property);
        }

        /// <summary>
        /// 调用构造函数
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns></returns>
        public static PropertyHandler Create(PropertyInfo property) => new PropertyHandler(property);

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; }
        /// <summary>
        /// 属性的Get方法委托
        /// </summary>
        public Func<object, object> Get { get; }
        /// <summary>
        /// 属性的Set方法委托
        /// </summary>
        public Action<object, object> Set { get; }
        /// <summary>
        /// 属性
        /// </summary>
        public PropertyInfo Property { get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <param name="target">属性实例对象</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        public bool SetValue(ConvertContext context, object target, object value)
        {
            if (Set == null)
            {
                context.AddException($"{Property.ReflectedType}.{Property.Name}属性没有set");
                return false;
            }
            bool b;
            var v = context.Get(PropertyType).ChangeType(context, value, PropertyType, out b);
            if (b == false)
            {
                context.AddException($"{Property.ReflectedType}属性{Property.Name}值转换失败");
                return false;
            }
            try
            {
                Set(target, v);
                return true;
            }
            catch (Exception ex)
            {
                context.AddException(ex);
                return false;
            }
        }
    }
}