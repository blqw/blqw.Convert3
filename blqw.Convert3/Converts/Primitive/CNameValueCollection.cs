using System;
using System.Collections;
using System.Collections.Specialized;
using blqw.IOC;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="NameValueCollection" /> 转换器
    /// </summary>
    public class CNameValueCollection : BaseTypeConvertor<NameValueCollection>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected override NameValueCollection ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            if (input.IsNull())
            {
                success = true;
                return null;
            }
            var builder = new NVCollectiontBuilder(context, outputType);
            if (builder.TryCreateInstance() == false)
            {
                success = false;
                return null;
            }

            var mapper = new Mapper(input);

            if (mapper.Error != null)
            {
                context.AddException(mapper.Error);
                success = false;
                return null;
            }

            while (mapper.MoveNext())
            {
                if (builder.Add(mapper.Key, mapper.Value) == false)
                {
                    success = false;
                    return null;
                }
            }

            success = true;
            return builder.Instance;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override NameValueCollection ChangeType(ConvertContext context, string input, Type outputType,
            out bool success)
        {
            input = input?.Trim();
            if ((input?.Length > 1) && (input[0] == '{') && (input[input.Length - 1] == '}'))
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (NameValueCollection) result;
                }
                catch (Exception ex)
                {
                    context.AddException(ex);
                }
            }
            success = false;
            return null;
        }


        /// <summary>
        /// <seealso cref="NameValueCollection" /> 转换器
        /// </summary>
        private struct NVCollectiontBuilder : IBuilder<NameValueCollection, DictionaryEntry>
        {
            private readonly ConvertContext _context;
            private readonly Type _type;

            public NVCollectiontBuilder(ConvertContext context, Type type)
            {
                _context = context;
                _type = type;
                Instance = null;
            }

            /// <summary>
            /// 被构造的实例
            /// </summary>
            public NameValueCollection Instance { get; private set; }

            /// <summary>
            /// 设置对象值
            /// </summary>
            /// <param name="obj"> 待设置的值 </param>
            /// <returns> </returns>
            public bool Set(DictionaryEntry obj) => Add(obj.Key, obj.Value);

            /// <summary>
            /// 尝试构造实例,返回是否成功
            /// </summary>
            /// <returns> </returns>
            public bool TryCreateInstance()
            {
                try
                {
                    Instance = (NameValueCollection) Activator.CreateInstance(_type);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException(ex);
                    return false;
                }
            }

            public bool Add(object key, object value)
            {
                var conv = _context.Get<string>();
                bool b;
                var skey = conv.ChangeType(_context, key, typeof(string), out b);
                if (b == false)
                {
                    return false;
                }
                var svalue = conv.ChangeType(_context, value, typeof(string), out b);
                if (b == false)
                {
                    return false;
                }
                try
                {
                    Instance.Add(skey, svalue);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException(ex);
                    return false;
                }
            }
        }
    }
}