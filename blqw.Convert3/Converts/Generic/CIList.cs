using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using blqw.IOC;

namespace blqw.Converts
{
    /// <summary>
    /// 泛型<see cref="IList{T}" />转换器
    /// </summary>
    /// <typeparam name="T"> 集合的元素类型 </typeparam>
    public class CIList<T> : BaseTypeConvertor<ICollection<T>>
    {
        /// <summary>
        /// 用于在字符串中拆分数组元素的分隔符
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly string[] _Separator = { ", ", "," };

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected override ICollection<T> ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            success = true;
            if (input.IsNull())
            {
                return null;
            }
            var builder = new ListBuilder(context, outputType);
            if (builder.TryCreateInstance() == false)
            {
                success = false;
                return null;
            }
            var reader = input as IDataReader;
            if (reader != null)
            {
                if (reader.IsClosed)
                {
                    context.AddException("DataReader已经关闭");
                    success = false;
                    return null;
                }
                while (reader.Read())
                {
                    if (builder.Set(reader))
                    {
                        continue;
                    }
                    success = false;
                    return null;
                }
                return builder.Instance;
            }

            var ee = (input as IEnumerable)?.GetEnumerator()
                     ?? input as IEnumerator
                     ?? (input as DataTable)?.Rows.GetEnumerator()
                     ?? (input as DataRow)?.ItemArray.GetEnumerator()
                     ?? (input as DataRowView)?.Row.ItemArray.GetEnumerator()
                     ?? (input as IListSource)?.GetList()?.GetEnumerator();

            if (ee == null)
            {
                if (builder.Set(input))
                {
                    return builder.Instance;
                }
                success = false;
                return null;
            }

            while (ee.MoveNext())
            {
                if (builder.Set(ee.Current))
                {
                    continue;
                }
                success = false;
                return null;
            }
            return builder.Instance;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override ICollection<T> ChangeType(ConvertContext context, string input, Type outputType,
            out bool success)
        {
            input = input.Trim();
            if (input.Length == 0)
            {
                var builder = new ListBuilder(context, outputType);
                // ReSharper disable once AssignmentInConditionalExpression
                return (success = builder.TryCreateInstance()) ? builder.Instance : null;
            }
            if ((input[0] == '[') && (input[input.Length - 1] == ']'))
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (ICollection<T>) result;
                }
                catch (Exception ex)
                {
                    context.AddException(ex);
                    success = false;
                    return null;
                }
            }
            var arr = input.Split(_Separator, StringSplitOptions.None);
            return ChangeType(context, arr, outputType, out success);
        }

        /// <summary>
        /// 泛型 <see cref="IList{T}" /> 构造器
        /// </summary>
        protected struct ListBuilder : IBuilder<ICollection<T>, object>
        {
            /// <summary>
            /// 被构造的实例
            /// </summary>
            public ICollection<T> Instance { get; private set; }

            /// <summary>
            /// 值转换器
            /// </summary>
            private readonly IConvertor<T> _convertor;

            /// <summary>
            /// 转换上下文
            /// </summary>
            private readonly ConvertContext _context;

            /// <summary>
            /// 需要构造的实例类型
            /// </summary>
            private readonly Type _type;

            /// <summary>
            /// 初始化构造器
            /// </summary>
            /// <param name="context"> 转换上下文 </param>
            /// <param name="type"> 需要构造的实例类型 </param>
            public ListBuilder(ConvertContext context, Type type)
            {
                _context = context;
                _type = type;
                _convertor = context.Get<T>();
                Instance = null;
            }

            /// <summary>
            /// 设置对象值
            /// </summary>
            /// <param name="obj"> 待设置的值 </param>
            /// <returns> </returns>
            public bool Set(object obj)
            {
                bool b;
                var v = _convertor.ChangeType(_context, obj, _convertor.OutputType, out b);
                if (b == false)
                {
                    _context.AddException($"向集合{CType.GetFriendlyName(_type)}中添加第[{Instance?.Count}]个元素失败");
                    return false;
                }
                try
                {
                    Instance.Add(v);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException($"向集合{CType.GetFriendlyName(_type)}中添加第[{Instance?.Count}]个元素失败,原因:{ex.Message}", ex);
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
                    Instance = new List<T>();
                    return true;
                }
                try
                {
                    Instance = (ICollection<T>) Activator.CreateInstance(_type);
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