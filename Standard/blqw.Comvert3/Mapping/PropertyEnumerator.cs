namespace blqw.Mapping
{
    /// <summary>
    /// 属性枚举器
    /// </summary>
    internal struct PropertyEnumerator : IKeyValueEnumerator
    {
        private readonly object _entity;
        private readonly PropertyHandler[] _properties;
        private readonly int _propertyCount;
        private int _index;
        private PropertyHandler _currentProperty;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="entity"> </param>
        /// <param name="properties"> </param>
        public PropertyEnumerator(object entity, PropertyHandler[] properties)
        {
            _entity = entity;
            _properties = properties;
            _index = -1;
            _propertyCount = _properties.Length;
            _currentProperty = null;
            Error = null;
        }


        /// <summary>
        /// 异常消息
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// 将枚举数推进到集合的下一个元素。
        /// </summary>
        /// <returns> 如果枚举数成功地推进到下一个元素，则为 true；如果枚举数越过集合的结尾，则为 false。 </returns>
        /// <exception cref="T:System.InvalidOperationException"> 在创建了枚举数后集合被修改了。 </exception>
        public bool MoveNext()
        {
            while (++_index < _propertyCount)
            {
                _currentProperty = _properties[_index];
                if (_currentProperty.Get == null)
                {
                    continue;
                }
                return true;
            }
            _currentProperty = null;
            return false;
        }


        /// <summary>
        /// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"> 在创建了枚举数后集合被修改了。 </exception>
        public void Reset() => _index = -1;

        /// <summary>
        /// 获取集合中的当前元素的Key
        /// </summary>
        /// <returns> </returns>
        public object GetKey() => _currentProperty.Name;

        /// <summary>
        /// 获取集合中的当前元素的Value
        /// </summary>
        /// <returns> </returns>
        public object GetValue() => _currentProperty.Get(_entity);
    }
}