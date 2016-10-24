using System.Data;

namespace blqw.Mapping
{
    /// <summary>
    /// <seealso cref="DataRow"/> 枚举器
    /// </summary>
    internal struct DataRowEnumerator : IKeyValueEnumerator
    {
        private readonly DataRow _row;
        private int _index;
        private readonly int _columnCount;
        private readonly DataColumnCollection _columns;
        private DataColumn _cuuentColumn;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="row"></param>
        public DataRowEnumerator(DataRow row)
        {
            _row = row;
            _index = -1;
            _columns = row.Table.Columns;
            _columnCount = _columns.Count;
            _cuuentColumn = null;
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
            if (++_index < _columnCount)
            {
                _cuuentColumn = _columns[_index];
                return true;
            }
            _cuuentColumn = null;
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
        public object GetKey() => _cuuentColumn?.ColumnName;

        /// <summary>
        /// 获取集合中的当前元素的Value
        /// </summary>
        /// <returns> </returns>
        public object GetValue() => _row[_cuuentColumn];
    }
}