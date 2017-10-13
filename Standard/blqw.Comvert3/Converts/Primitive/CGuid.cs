using System;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="Guid" /> 转换器
    /// </summary>
    public class CGuid : SystemTypeConvertor<Guid>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override Guid ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            if (input.Length == 0)
            {
                success = false;
                return Guid.Empty;
            }
            Guid result;
            success = Guid.TryParse(input, out result);
            return result;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override Guid ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            var bytes = input as byte[];
            if (bytes?.Length == 16)
            {
                success = true;
                return new Guid(bytes);
            }
            if (input is decimal)
            {
                var arr = decimal.GetBits((decimal) input);
                bytes = new byte[16];
                Buffer.BlockCopy(arr, 0, bytes, 0, 16);
                success = true;
                return new Guid(bytes);
            }
            success = false;
            return default(Guid);
        }
    }
}