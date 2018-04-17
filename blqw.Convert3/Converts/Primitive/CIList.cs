using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using blqw.IOC;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="IList" /> 构造器
    /// </summary>
    public class CIList : BaseTypeConvertor<IList>
    {
        /// <summary>
        /// 字符串分隔符
        /// </summary>
        private static readonly string[] _Separator = { ", ", "," };

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected override IList ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            if (input.IsNull())
            {
                return null;
            }

            var helper = new ListBuilder(outputType, context);
            if (helper.TryCreateInstance() == false)
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
                    var dict = (IDictionary<string, object>) new ExpandoObject();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        dict[reader.GetName(i)] = reader.GetValue(i);
                    }
                    helper.Set(dict);
                }

                return helper.Instance;
            }

            var ee = (input as IEnumerable)?.GetEnumerator()
                     ?? input as IEnumerator
                     ?? (input as DataTable)?.Rows.GetEnumerator()
                     ?? (input as DataRow)?.ItemArray.GetEnumerator()
                     ?? (input as DataRowView)?.Row.ItemArray.GetEnumerator()
                     ?? (input as IListSource)?.GetList()?.GetEnumerator();

            if (ee == null)
            {
                if (helper.Set(input))
                {
                    return helper.Instance;
                }
                success = false;
                return null;
            }

            while (ee.MoveNext())
            {
                if (helper.Set(ee.Current) == false)
                {
                    success = false;
                    return null;
                }
            }
            return helper.Instance;
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override IList ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            input = input?.Trim();
            if ((input == null) || (input.Length <= 1))
            {
                success = false;
                return null;
            }
            if ((input[0] == '[') && (input[input.Length - 1] == ']'))
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (IList) result;
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
        /// <seealso cref="IList" /> 构造器
        /// </summary>
        private struct ListBuilder : IBuilder<IList, object>
        {
            private readonly Type _type;
            private readonly ConvertContext _context;

            public ListBuilder(Type type, ConvertContext context)
            {
                _type = type;
                _context = context;
                Instance = null;
            }

            /// <summary>
            /// 被构造的实例
            /// </summary>
            public IList Instance { get; private set; }

            /// <summary>
            /// 设置对象值
            /// </summary>
            /// <param name="value"> 待设置的值 </param>
            /// <returns> </returns>
            public bool Set(object value)
            {
                try
                {
                    Instance.Add(value);
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
                    Instance = new ArrayList();
                    return true;
                }
                try
                {
                    Instance = (IList) Activator.CreateInstance(_type);
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