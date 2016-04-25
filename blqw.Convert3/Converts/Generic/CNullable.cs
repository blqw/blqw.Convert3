using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CNullable : SystemTypeConvertor<ValueType>, IConvertor
    {
        protected override ValueType ChangeType(string input, Type outputType, out bool success)
        {
            throw new NotImplementedException();
        }

        protected override ValueType ChangeType(object input, Type outputType, out bool success)
        {
            throw new NotImplementedException();
        }

        IConvertor IConvertor.GetConvertor(Type outputType)
        {
            var valuetype = Nullable.GetUnderlyingType(outputType);
            if (valuetype == null)
            {
                throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}必须是可空值类型");
            }
            var type = typeof(CNullable<>).MakeGenericType(valuetype);
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }
    }

    public class CNullable<T> : SystemTypeConvertor<T?>
        where T : struct
    {
        IConvertor<T> _conv;
        Type _type = typeof(T);

        protected override void Initialize()
        {
            _conv = ConvertorContainer.Default.Get<T>();
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

    }
}
