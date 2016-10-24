using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;

namespace blqw.Mapping
{
    /// <summary>
    /// <seealso cref="IDataReader"/> 枚举器
    /// </summary>
    internal struct DataReaderEnumerator : IKeyValueEnumerator
    {
        /// <summary>
        /// 待枚举的 <seealso cref="IDataReader"/>
        /// </summary>
        private readonly IDataReader _reader;
        /// <summary>
        /// 当前循环索引
        /// </summary>
        private int _index;
        /// <summary>
        /// <seealso cref="IDataReader"/> 总列数
        /// </summary>
        private readonly int _fieldCount;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="reader"></param>
        public DataReaderEnumerator(IDataReader reader)
            : this()
        {
            if (reader.IsClosed)
            {
                Error = "DataReader已经关闭";
                return;
            }
            _reader = reader;
            _index = -1;
            _fieldCount = reader.FieldCount;
        }

        /// <summary>
        /// 异常
        /// </summary>
        public string Error { get; }

        /// <summary> 将枚举数推进到集合的下一个元素。 </summary>
        /// <returns> 如果枚举数成功地推进到下一个元素，则为 true；如果枚举数越过集合的结尾，则为 false。 </returns>
        /// <exception cref="T:System.InvalidOperationException"> 在创建了枚举数后集合被修改了。 </exception>
        public bool MoveNext() => ++_index < _fieldCount;

        /// <summary> 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。 </summary>
        /// <exception cref="T:System.InvalidOperationException"> 在创建了枚举数后集合被修改了。 </exception>
        public void Reset() => _index = -1;

        /// <summary>
        /// 获取集合中的当前元素的Key
        /// </summary>
        /// <returns> </returns>
        public object GetKey() => _reader.GetName(_index);

        /// <summary>
        /// 获取集合中的当前元素的Value
        /// </summary>
        /// <returns> </returns>
        public object GetValue() => _reader.GetValue(_index);
    }
}