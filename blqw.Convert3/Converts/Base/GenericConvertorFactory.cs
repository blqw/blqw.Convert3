using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace blqw
{
    sealed class GenericConvertorFactory : IConvertor
    {
        Type _convType;
        public GenericConvertorFactory(Type convType)
        {
            _convType = convType;
        }

        public uint Priority
        {
            get { return uint.MaxValue; }
        }

        public Type OutputType
        {
            get { return typeof(Type); }
        }

        public bool Try(object input, Type outputType, out object result)
        {
            if (outputType == null)
            {
                outputType = input as Type;
                if (outputType == null)
                {
                    result = null;
                    return false;
                }
            }
            if (outputType.IsGenericType == false
                || outputType.IsGenericTypeDefinition == false)
            {
                result = null;
                return false;
            }
            //outputType.GenericTypeArguments
            throw new NotImplementedException();
        }

        public bool Try(string input, Type outputType, out object result)
        {
            result = null;
            return false;
        }

        public Type Create(Type outputType)
        {
            return _convType.MakeGenericType(outputType.GetGenericArguments());
        }

        void IConvertor.Initialize() { }

    }
}
