using System.Reflection;
using blqw.Define;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 自定义转换方法参数
    /// </summary>
    public struct CustomChangeTypeArg
    {

        /// <summary>
        /// 自定义转换方法参数
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <param name="inputValue">输入值</param>
        /// <param name="outputType">输出类型</param>
        public CustomChangeTypeArg(IConvertContext context, object inputValue, Type outputType)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            OutputType = outputType ?? throw new ArgumentNullException(nameof(outputType));
            InputValue = inputValue;
        }
        /// <summary>
        /// 上下文
        /// </summary>
        public IConvertContext Context { get; }
        /// <summary>
        /// 输入值
        /// </summary>
        public object InputValue { get; }
        
        /// <summary>
        /// 输出类型
        /// </summary>
        public Type OutputType { get; }

        /// <summary>
        /// 转换失败
        /// </summary>
        public object Fail() => ConvertFailed.Instance;

        /// <summary>
        /// 判断是否转换失败
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsFail(object value) => value is ConvertFailed;
    }
    

    /// <summary>
    /// 自定义转换方法
    /// </summary>
    /// <param name="context"></param>
    /// <param name="input"></param>
    /// <param name="outputType"></param>
    /// <param name="success"></param>
    /// <returns></returns>
    public delegate object CustomChangeTypeHandler(CustomChangeTypeArg arg);
}
