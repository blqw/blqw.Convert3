using System;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="Uri" /> 转换器
    /// </summary>
    public class CUri : SystemTypeConvertor<Uri>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override Uri ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            if (input == null)
            {
                success = true;
                return null;
            }
            Uri result;
            input = input.TrimStart();
            if ((input.Length > 10) && (input[6] != '/'))
            {
                if (Uri.TryCreate("http://" + input, UriKind.Absolute, out result))
                {
                    success = true;
                    return result;
                }
            }

            if (Uri.TryCreate(input, UriKind.Absolute, out result))
            {
                success = true;
                return result;
            }

            context.AddException(input + "不是一个有效的url");
            success = false;
            return null;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override Uri ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            success = false;
            return null;
        }
    }
}