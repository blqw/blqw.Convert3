using System;
using System.Collections.Specialized;
using System.Reflection;

namespace blqw.Mapping
{
    /// <summary>
    /// <seealso cref="NameValueCollection" /> 枚举器
    /// </summary>
    internal struct NameValueEnumerator : IKeyValueEnumerator
    {

        private static Func<NameObjectCollectionBase, int, string> BaseGetKey =
            (Func<NameObjectCollectionBase, int, string>)typeof(NameObjectCollectionBase).GetMethod("BaseGetKey", BindingFlags.Instance | BindingFlags.NonPublic).CreateDelegate(typeof(Func<NameObjectCollectionBase, int, string>));
        private static Func<NameObjectCollectionBase, int, object> BaseGet =
            (Func<NameObjectCollectionBase, int, object>)typeof(NameObjectCollectionBase).GetMethod("BaseGet", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(int) }, null).CreateDelegate(typeof(Func<NameObjectCollectionBase, int, object>));


        private readonly NameObjectCollectionBase _collection;
        private int _index;
        private readonly int _count;
        private readonly NameValueCollection _nv;
        /// <summary>
        /// 初始化 
        /// </summary>
        /// <param name="collection"></param>
        public NameValueEnumerator(NameObjectCollectionBase collection)
        {
            _collection = collection;
            _nv = collection as NameValueCollection;
            _index = -1;
            _count = collection.Count;
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
        public bool MoveNext()
        {
            _index += 1;
            return _index < _count;
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
        public object GetKey() => BaseGetKey(_collection, _index);

        /// <summary>
        /// 获取集合中的当前元素的Value
        /// </summary>
        /// <returns> </returns>
        public object GetValue() => _nv != null ? _nv.Get(_index) : BaseGet(_collection, _index);
    }
}