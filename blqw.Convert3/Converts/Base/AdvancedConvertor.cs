using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 
    /// 高级转换器,提供高级类型的转换器基类
    /// </summary>
    /// <typeparam name="T">高级类型泛型</typeparam>
    /// <remarks>
    /// 高级类型的定义: T 接口,抽象类,或可以被继承的类型,
    /// outputType 不一定完全等于 typeof(T),
    /// 例如:数组,只需要实现Array的转换,并不用实现每一个 T[]
    /// </remarks>
    public abstract class AdvancedConvertor<T> : BaseConvertor<T>, IConvertor
    {
        protected override T ChangeTypeImpl(object input, Type outputType, out bool success)
        {
            if (outputType.IsInstanceOfType(input))
            {
                success = true;
                return (T)input;
            }
            if (outputType.IsGenericTypeDefinition)
            {
                Error.CastFail("无法转为泛型定义类");
                success = false;
                return default(T);
            }
            return ChangeType(input, outputType, out success);
        }
        
        protected virtual IConvertor GetConvertor(Type outputType)
        {
            return this;
        }

        IConvertor IConvertor.GetConvertor(Type outputType)
        {
            if (outputType == null)
                throw new ArgumentNullException(nameof(outputType));
            if (OutputType.IsAssignableFrom(outputType) == false)
                throw new ArgumentOutOfRangeException(nameof(outputType),$"类型{outputType}不是{OutputType}的子类或实现类");
            return GetConvertor(outputType);
        }
    }
}
