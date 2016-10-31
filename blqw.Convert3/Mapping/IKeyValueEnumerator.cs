namespace blqw.Mapping
{
    /// <summary>
    /// 提供获取当前元素键值方法的枚举器
    /// </summary>
    internal interface IKeyValueEnumerator
    {
        /// <summary>
        /// 将枚举数推进到集合的下一个元素。
        /// </summary>
        /// <returns> 如果枚举数成功地推进到下一个元素，则为 true；如果枚举数越过集合的结尾，则为 false。 </returns>
        /// <exception cref="T:System.InvalidOperationException"> 在创建了枚举数后集合被修改了。 </exception>
        bool MoveNext();

        /// <summary>
        /// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"> 在创建了枚举数后集合被修改了。 </exception>
        void Reset();

        /// <summary>
        /// 获取集合中的当前元素的Key
        /// </summary>
        /// <returns> </returns>
        object GetKey();

        /// <summary>
        /// 获取集合中的当前元素的Value
        /// </summary>
        /// <returns> </returns>
        object GetValue();

        /// <summary>
        /// 异常消息
        /// </summary>
        string Error { get; }
    }
}