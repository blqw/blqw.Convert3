using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    sealed class CNullable<T> : SystemTypeConvertor<T?>
        where T : struct
    {
        IConvertor<T> _conv;
        Type _type;

        protected override void Initialize()
        {
            _conv = Convert3.GetConvertor<T>();
            _type = typeof(T);
        }

        protected override bool Try(object input, out T? result)
        {
            if (input == null || input is DBNull)
            {
                result = null;
                return true;
            }

            T t;
            if (_conv == null)
            {
                if (CObject.TryTo<T>(input, _type, out t))
                {
                    result = t;
                    return true;
                }
            }
            else if (_conv.Try(input, _type, out t))
            {
                result = t;
                return true;
            }
            result = default(T);
            return false;
        }

        protected override bool Try(string input, out T? result)
        {
            if (input.Length == 0)
            {
                result = null;
                return true;
            }

            T t;
            if (_conv == null)
            {
                if (CObject.TryTo<T>(input, _type, out t))
                {
                    result = t;
                    return true;
                }
            }
            else if (_conv.Try(input, _type, out t))
            {
                result = t;
                return true;
            }
            result = default(T);
            return false;
        }
    }
}
