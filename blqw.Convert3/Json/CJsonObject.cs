using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.Convert3Component;

namespace blqw
{
    class CJsonObject : CObject
    {
        public override uint Priority
        {
            get
            {
                return base.Priority + 1;
            }
        }

        protected override bool Try(string input, Type outputType, out object result)
        {
            if (input != null && input.Length > 2)
            {
                switch (input[0])
                {
                    case '"':
                        if (input[input.Length - 1] != '"')
                            return base.Try(input, outputType, out result);
                        break;
                    case '\'':
                        if (input[input.Length - 1] != '\'')
                            return base.Try(input, outputType, out result);
                        break;
                    case '{':
                        if (input[input.Length - 1] != '}')
                            return base.Try(input, outputType, out result);
                        break;
                    case '[':
                        if (input[input.Length - 1] != ']')
                            return base.Try(input, outputType, out result);
                        break;
                    default:
                        return base.Try(input, outputType, out result);
                }
                try
                {
                    result = Component.ToJsonObject(outputType, input);
                    return true;
                }
                catch(Exception ex)
                {
                    ErrorContext.Error = ex;
                }
            }
            return base.Try(input, outputType, out result);
        }

        static CJsonObject Convertor = new CJsonObject();
        /// <summary> 尝试将指定对象转换为指定类型的值。返回是否转换成功
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="result">如果转换成功,则包含转换后的对象,否则为default(T)</param>
        public static bool TryTo<T>(string input, Type outputType, out T result)
        {
            object r;
            if (Convertor.Try(input, outputType, out r))
            {
                result = (T)r;
                return true;
            }
            result = default(T);
            return false;
        }
    }
}
