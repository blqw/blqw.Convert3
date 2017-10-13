using System;
using System.Collections.Generic;
using System.Dynamic;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="object" /> 转换器
    /// </summary>
    public class CObject : BaseTypeConvertor<object>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected override object ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            if ((input == null) || input is DBNull)
            {
                success = true;
                return null;
            }
            var builder = new ObjectBuilder(context, outputType);
            if (builder.TryCreateInstance() == false)
            {
                return ReturnIfObject(input, outputType, out success);
            }

            var mapper = new Mapper(input);

            if (mapper.Error != null)
            {
                return ReturnIfObject(input, outputType, out success);
            }

            while (mapper.MoveNext())
            {
                var name = mapper.Key as string;
                if ((name != null) && (builder.Set(name, mapper.Value) == false))
                {
                    return ReturnIfObject(input, outputType, out success);
                }
            }

            success = true;
            return builder.Instance;
        }

        private object ReturnIfObject(object input, Type outputType, out bool success)
        {
            if (outputType == typeof(object))
            {
                success = true;
                return input;
            }
            success = false;
            return null;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override object ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            try
            {
                if (outputType == typeof(object))
                {
                    success = true;
                    return input;
                }
                var type = Type.GetType(input, false, true);
                if (type == null)
                {
                    success = false;
                    return null;
                }
                if (outputType.IsAssignableFrom(type) == false)
                {
                    success = false;
                    return null;
                }

                success = true;
                return Activator.CreateInstance(type, true);
            }
            catch (Exception ex)
            {
                context.AddException(ex);
            }
            success = false;
            return null;
        }


        protected struct ObjectBuilder : IBuilder<object, KeyValuePair<string, object>>
        {
            private readonly IConvertContext _context;
            private readonly PropertyHandler[] _properties;
            private readonly int _propertyCount;

            private IDictionary<string, object> _dynamic;
            private readonly Type _type;

            public ObjectBuilder(IConvertContext context, Type type)
            {
                _context = context;
                if ((type == typeof(object)) || (type == null))
                {
                    _type = null;
                    _properties = null;
                    _propertyCount = 0;
                }
                else
                {
                    _properties = PublicPropertyCache.GetByType(type);
                    _propertyCount = _properties.Length;
                    _type = type;
                }
                _dynamic = null;
                Instance = null;
            }

            /// <summary>
            /// 被构造的实例
            /// </summary>
            public object Instance { get; private set; }

            /// <summary>
            /// 设置对象值
            /// </summary>
            /// <param name="obj"> 待设置的值 </param>
            /// <returns> </returns>
            public bool Set(KeyValuePair<string, object> obj)
                => Set(obj.Key, obj.Value);

            /// <summary>
            /// 尝试构造实例,返回是否成功
            /// </summary>
            /// <returns> </returns>
            public bool TryCreateInstance()
            {
                if (_type == null)
                {
                    Instance = _dynamic = new ExpandoObject();
                    return true;
                }
                if (_type.IsInterface)
                {
                    _context.AddException($"无法创建该接口的实例({_type})");
                    return false;
                }
                if (_type.IsAbstract)
                {
                    if (_type.IsSealed)
                    {
                        _context.AddException("无法创建静态类实例");
                    }
                    else
                    {
                        _context.AddException("无法创建抽象类实例");
                    }
                    return false;
                }
                try
                {
                    Instance = Activator.CreateInstance(_type);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException(ex);
                    return false;
                }
            }

            private PropertyHandler GetProperty(string name)
            {
                for (var i = 0; i < _propertyCount; i++)
                {
                    var p = _properties[i];
                    if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return p;
                    }
                }
                return null;
            }

            public bool Set(string name, object value)
            {
                if (_dynamic != null)
                {
                    _dynamic[name] = value;
                    return true;
                }
                var p = GetProperty(name);
                if (p?.Set != null)
                {
                    return p.SetValue(_context, Instance, value);
                }
                return true;
            }
        }
    }
}