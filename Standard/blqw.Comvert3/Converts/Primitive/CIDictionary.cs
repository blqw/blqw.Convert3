using System;
using System.Collections;


namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="IDictionary" /> 转换器
    /// </summary>
    public class CIDictionary : BaseTypeConvertor<IDictionary>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected override IDictionary ChangeTypeImpl(IConvertContext context, object input, Type outputType,
            out bool success)
        {
            if ((input == null) || input is DBNull)
            {
                success = true;
                return null;
            }

            var builder = new DictionaryBuilder(outputType, context);
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
        protected override IDictionary ChangeType(IConvertContext context, string input, Type outputType,
            out bool success)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                success = false;
                return null;
            }
            input = input.Trim();
            if ((input[0] == '{') && (input[input.Length - 1] == '}'))
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (IDictionary) result;
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
        /// <seealso cref="IDictionary" /> 构造器
        /// </summary>
        protected struct DictionaryBuilder : IBuilder<IDictionary, DictionaryEntry>
        {
            private readonly Type _type;
            private readonly IConvertContext _context;

            public DictionaryBuilder(Type type, IConvertContext context)
            {
                _type = type;
                _context = context;
                Instance = null;
            }

            /// <summary>
            /// 被构造的实例
            /// </summary>
            public IDictionary Instance { get; private set; }

            public bool Add(object key, object value)
            {
                try
                {
                    Instance.Add(key, value);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException($"向字典{CType.GetFriendlyName(_type)}中添加元素 {key} 失败,原因:{ex.Message}", ex);
                    return false;
                }
            }

            /// <summary>
            /// 尝试构造实例,返回是否成功
            /// </summary>
            /// <returns> </returns>
            public bool TryCreateInstance()
            {
                if (_type.IsInterface)
                {
                    Instance = new Hashtable();
                    return true;
                }
                try
                {
                    Instance = (IDictionary) Activator.CreateInstance(_type);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException(ex);
                    return false;
                }
            }

            /// <summary>
            /// 设置对象值
            /// </summary>
            /// <param name="obj"> 待设置的值 </param>
            /// <returns> </returns>
            public bool Set(DictionaryEntry obj) => Add(obj.Key, obj.Value);
        }
    }
}