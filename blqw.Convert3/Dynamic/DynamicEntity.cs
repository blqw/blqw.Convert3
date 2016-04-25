using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Dynamic
{
    class DynamicEntity : DynamicObject, IFormatProvider
    {
        #region IFormatProvider 成员

        object IFormatProvider.GetFormat(Type formatType)
        {
            if (formatType != null && string.Equals("Json", formatType.Name, StringComparison.Ordinal))
            {
                return _Entity;
            }
            return null;
        }

        #endregion

        object _Entity;
        int _PropertyCount;
        PropertyHandler[] _Properties;

        public DynamicEntity(object entity)
        {
            if (entity == null) return;
            _Entity = entity;
            _Properties = PublicPropertyCache.GetByType(entity.GetType());
            _PropertyCount = _Properties.Length;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            for (int i = 0; i < _PropertyCount; i++)
            {
                yield return _Properties[i].Name;
            }
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return Convert3.TryChangedType(_Entity, binder.ReturnType, out result);
        }

        private PropertyHandler this[string name]
        {
            get
            {
                for (int i = 0; i < _PropertyCount; i++)
                {
                    var p = _Properties[i];
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
            if (p != null && p.Get != null)
            {
                result = p.Get(_Entity);
                if (Convert3.TryChangedType(result, binder.ReturnType, out result))
                {
                    result = Convert3.ToDynamic(result);
                    return true;
                }
            }
            result = DynamicSystemObject.Null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var p = this[binder.Name];
            if (p == null)
            {
                return false;
            }
            return p.SetValue(_Entity, value);
        }


    }
}
