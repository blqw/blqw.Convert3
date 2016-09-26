using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CNullable : CNullable<int>
    {
        public override Type OutputType => typeof(Nullable<>);
    }

    public class CNullable<T> : GenericConvertor<T?>
        where T : struct
    {
        IConvertor<T> _conv;
        Type _type = typeof(T);

        protected override void Initialize()
        {
            _conv = ConvertorServices.Container.GetConvertor<T>();
        }


        protected override T? ChangeType(string input, Type outputType, out bool success)
        {
            if (input.Length == 0)
            {
                success = true;
                return null;
            }

            if (_conv == null)
            {
                Error.ConvertorNotFound(typeof(T));
                success = false;
                return null;
            }

            return _conv.ChangeType(input, _conv.OutputType, out success);
        }

        protected override T? ChangeType(object input, Type outputType, out bool success)
        {
            if (input == null || input is DBNull)
            {
                success = true;
                return null;
            }

            if (_conv == null)
            {
                Error.ConvertorNotFound(typeof(T));
                success = false;
                return null;
            }
            return _conv.ChangeType(input, _conv.OutputType, out success);
        }

        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CNullable<>).MakeGenericType(genericTypes);
            return (IConvertor)Activator.CreateInstance(type);
        }
    }
}
