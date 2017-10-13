using System;

namespace blqw.Converts
{
    /// <summary>
    /// 用于定义转换器工厂基类
    /// </summary>
    public abstract class ConvertorFactory : BaseConvertor<object>
    {
        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        public abstract override Type OutputType { get; }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override object ChangeType(IConvertContext context, object input, Type outputType, out bool success) => throw new NotImplementedException();

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override object ChangeType(IConvertContext context, string input, Type outputType, out bool success) => throw new NotImplementedException();

        /// <summary>
        /// 获取子转换器
        /// </summary>
        protected abstract override IConvertor GetConvertor(Type outputType);
    }
}