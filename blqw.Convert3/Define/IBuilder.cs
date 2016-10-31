namespace blqw
{
    /// <summary>
    /// 对象构造器接口
    /// </summary>
    internal interface IBuilder<out TInstance, in TValue>
    {
        /// <summary>
        /// 被构造的实例
        /// </summary>
        TInstance Instance { get; }

        /// <summary>
        /// 设置对象值
        /// </summary>
        /// <param name="obj"> 待设置的值 </param>
        /// <returns> </returns>
        bool Set(TValue obj);

        /// <summary>
        /// 尝试构造实例,返回是否成功
        /// </summary>
        /// <returns> </returns>
        bool TryCreateInstance();
    }
}