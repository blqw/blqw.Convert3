using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Dynamic
{
    class DynamicEntity : DynamicObject, IObjectHandle, IObjectReference
    {
        /// <summary>
        /// 返回应进行反序列化的真实对象（而不是序列化流指定的对象）。
        /// </summary>
        /// <returns>返回放入图形中的实际对象。</returns>
        /// <param name="context">当前对象从其中进行反序列化的 <see cref="T:System.Runtime.Serialization.StreamingContext" />。</param>
        public object GetRealObject(StreamingContext context) => _entity;


        private readonly object _entity;
        private readonly int _propertyCount;
        private readonly PropertyHandler[] _properties;

        public DynamicEntity(object entity)
        {
            if (entity == null) return;
            _entity = entity;
            _properties = PublicPropertyCache.GetByType(entity.GetType());
            _propertyCount = _properties.Length;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            for (int i = 0; i < _propertyCount; i++)
            {
                yield return _properties[i].Name;
            }
        }

        public override bool TryConvert(ConvertBinder binder, out object result) => Convert3.TryChangedType(_entity, binder.ReturnType, out result);

        private PropertyHandler this[string name]
        {
            get
            {
                for (int i = 0; i < _propertyCount; i++)
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

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var p = this[binder.Name];
            if (p?.Get != null)
            {
                result = p.Get(_entity);
                bool b;
                result = result.ChangeType(binder.ReturnType, out b);
                return b ? result.ToDynamic() : DynamicPrimitive.Null;
            }
            result = DynamicPrimitive.Null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var p = this[binder.Name];
            if (p == null)
            {
                return false;
            }
            return p.SetValue(ConvertContext.None, _entity, value);
        }


        /// <summary>打开该对象。</summary>
        /// <returns>已打开的对象。</returns>
        public object Unwrap() => _entity;
    }
}
