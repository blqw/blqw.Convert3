using System;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="TimeSpan"/> 转换器
    /// </summary>
    public class CTimeSpan : SystemTypeConvertor<TimeSpan>
    {
        /// <summary>
        /// 时间格式化字符
        /// </summary>
        private static readonly string[] _Formats = { "hhmmss", "hhmmssfff" };

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override TimeSpan ChangeTypeImpl(IConvertContext context, object input, Type outputType,
            out bool success)
        {
            success = false;
            return default(TimeSpan);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override TimeSpan ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            TimeSpan result;
            success = TimeSpan.TryParse(input, out result)
                      || TimeSpan.TryParseExact(input, _Formats, null, out result);
            if (success == false)
            {
                var number = context.Get<long>().ChangeType(context, input, typeof(long), out success);
                if (success)
                {
                    return TimeSpan.FromTicks(number);
                }
            }
            return result;
        }
    }
}