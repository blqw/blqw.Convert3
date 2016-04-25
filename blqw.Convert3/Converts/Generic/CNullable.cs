using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    //public class CNullable : SystemTypeConvertor<ValueType>, IConvertor
    //{
    //    protected override ValueType ChangeType(string input, Type outputType, out bool success)
    //    {
    //        Error.CastFail("无法为值类型(struct)提供转换");
    //        success = false;
    //        return null;
    //    }

    //    protected override ValueType ChangeType(object input, Type outputType, out bool success)
    //    {
    //        Error.CastFail("无法为值类型(struct)提供转换");
    //        success = false;
    //        return null;
    //    }

    //    IConvertor IConvertor.GetConvertor(Type outputType)
    //    {
    //        var valuetype = Nullable.GetUnderlyingType(outputType);
    //        if (valuetype == null)
    //        {
    //            throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}必须是可空值类型");
    //        }
    //        var type = typeof(CNullable<>).MakeGenericType(valuetype);
    //        var conv = (IConvertor)Activator.CreateInstance(type);
    //        return conv;
    //    }
    //}
    public class CNullable : CNullable<int>
    {
        public override Type OutputType
        {
            get
            {
                return typeof(Nullable<>);
            }
        }
    }
    public class CNullable<T> : GenericConvertor<T?>
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

        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CNullable<>).MakeGenericType(genericTypes);
            return (IConvertor)Activator.CreateInstance(type);
        }
    }
}
