using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{

    /// <summary> 转换器
    /// </summary>
    [InheritedExport(typeof(IConvertor))]
    public interface IConvertor
    {
        /// <summary>
        /// 初始化操作
        /// </summary>
        void Initialize();
        /// <summary> 
        /// 转换器优先级,只启用优先级最高的转换器
        /// </summary>
        /// <remarks>优先级相同不确定使用哪个</remarks>
        uint Priority { get; }
        /// <summary> 
        /// 转换器的输出类型
        /// (有可能是泛型定义类型)
        /// </summary>
        Type OutputType { get; }

        /// <summary>
        /// 输出类型的名称
        /// </summary>
        string OutputTypeName { get; }

        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        object ChangeType(object input, Type outputType, out bool success);
        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        object ChangeType(string input, Type outputType, out bool success);

        /// <summary>
        /// 根据<paramref name="outputType"/>返回一个<see cref="IConvertor"/>,<paramref name="outputType"/>应该是当前转换器<see cref="OutputType"/> 的兼容类型
        /// </summary>
        /// <param name="outputType">返回值类型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="outputType"/>不是<see cref="OutputType"/>的子类</exception>
        /// <exception cref="ArgumentNullException"><paramref name="outputType"/>是null</exception>
        IConvertor GetConvertor(Type outputType);
        
    }

    /// <summary> 泛型转换器
    /// </summary>
    /// <typeparam name="T">输出类型泛型</typeparam>
    public interface IConvertor<T> : IConvertor
    {
        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        new T ChangeType(object input, Type outputType, out bool success);
        /// <summary> 
        /// 返回指定类型的对象，其值等效于指定字符串。
        /// </summary>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success">是否成功</param>
        new T ChangeType(string input, Type outputType, out bool success);
    }

}
