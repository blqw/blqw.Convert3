using System;
using System.ComponentModel.Composition;
using blqw.IOC;

namespace blqw.Converts
{

    /// <summary>
    /// <seealso cref="object" /> 转换器,尝试将json字符串转为object对象
    /// </summary>
    [ExportMetadata("Priority", 1)]
    public sealed class CJsonObject : CObject
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override object ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            if (input?.Length > 2)
            {
                switch (input[0])
                {
                    case '"':
                        if (input[input.Length - 1] != '"')
                        {
                            return base.ChangeType(context, input, outputType, out success);
                        }
                        break;
                    case '\'':
                        if (input[input.Length - 1] != '\'')
                        {
                            return base.ChangeType(context, input, outputType, out success);
                        }
                        break;
                    case '{':
                        if (input[input.Length - 1] != '}')
                        {
                            return base.ChangeType(context, input, outputType, out success);
                        }
                        break;
                    case '[':
                        if (input[input.Length - 1] != ']')
                        {
                            return base.ChangeType(context, input, outputType, out success);
                        }
                        break;
                    default:
                        return base.ChangeType(context, input, outputType, out success);
                }
                try
                {
                    success = true;
                    return ComponentServices.ToJsonObject(outputType, input);
                }
                catch (Exception ex)
                {
                    context.AddException(ex);
                }
            }
            return base.ChangeType(context, input, outputType, out success);
        }
    }
}