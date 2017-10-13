using blqw.Define;
using System;

namespace blqw.Converts
{
    /// <summary>
    /// 基于自定义转换方法委托的转换器
    /// </summary>
    public sealed class DelegateConvertor : IConvertor
    {
        /// <summary>
        /// 自定义转换委托
        /// </summary>
        private readonly CustomChangeTypeHandler _handler;

        /// <summary>
        /// 基于自定义转换方法委托的转换器
        /// </summary>
        /// <param name="outputType">输出类型</param>
        /// <param name="handler"></param>
        public DelegateConvertor(Type outputType, CustomChangeTypeHandler handler)
        {
            OutputType = outputType;
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <inherit />
        public Type OutputType { get; }

        /// <inherit />
        public object ChangeType(IConvertContext context, object input, Type outputType, out bool success)
        {
            var snapshot = context.Snapshot(); //异常栈快照
            var result = _handler(new CustomChangeTypeArg(context, input, outputType));
            if (result is ConvertFailed == false) //转换成功 , 恢复异常栈
            {
                snapshot.Recovery();
                success = true;
                return result; 
            }
            if (snapshot.HasNewError == false)
            {
                context.AddCastFailException(input, outputType);
            }
            success = false;
            return null;
        }

        /// <inherit />
        public object ChangeType(IConvertContext context, string input, Type outputType, out bool success)
            => ChangeType(context, (object)input, outputType, out success);

        /// <inherit />
        public object GetService(Type serviceType) => this;
    }
}
