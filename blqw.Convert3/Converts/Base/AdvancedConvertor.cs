using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 
    /// 高级转换器,提供高级类型的转换器基类
    /// <para>高级类型的定义: T 接口,抽象类,或可以被继承的类型,
    /// outputType 不一定完全等于 typeof(T)</para>
    /// </summary>
    /// <typeparam name="T">高级类型泛型</typeparam>
    public abstract class AdvancedConvertor<T> : BaseConvertor<T>
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
    }
}
