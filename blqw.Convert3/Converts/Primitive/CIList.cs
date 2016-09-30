using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using blqw.IOC;

namespace blqw.Converts
{
    internal sealed class CIList : BaseTypeConvertor<IList>
    {
        private static readonly string[] _Separator = { ", ", "," };

        protected override IList ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            if ((input == null) || input is DBNull)
            {
                return null;
            }

            var helper = new ListHelper(outputType, context);
            if (helper.CreateInstance() == false)
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
                    var dict = (IDictionary<string, object>)new ExpandoObject();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        dict[reader.GetName(i)] = reader.GetValue(i);
                    }
                    helper.Add(dict);
                }

                return helper.List;
            }

            var ee = (input as IEnumerable)?.GetEnumerator()
                     ?? input as IEnumerator
                     ?? (input as DataTable)?.Rows.GetEnumerator()
                     ?? (input as DataRow)?.ItemArray.GetEnumerator()
                     ?? (input as DataRowView)?.Row.ItemArray.GetEnumerator()
                     ?? (input as IListSource)?.GetList()?.GetEnumerator();

            if (ee == null)
            {
                context.AddException("仅支持DataRow,DataRowView,或实现IEnumerator,IEnumerable,IListSource,IDataReader接口的对象对IList的转换");
                success = false;
                return null;
            }

            while (ee.MoveNext())
            {
                if (helper.Add(ee.Current) == false)
                {
                    success = false;
                    return null;
                }
            }
            return helper.List;
        }

        protected override IList ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            input = input?.Trim();
            if (input == null || input.Length <= 1)
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
                    return (IList)result;
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

        private struct ListHelper
        {
            public IList List;
            private readonly Type _type;
            private readonly ConvertContext _context;

            public ListHelper(Type type, ConvertContext context)
            {
                _type = type;
                _context = context;
                List = null;
            }

            public bool Add(object value)
            {
                try
                {
                    List.Add(value);
                    return true;
                }
                catch (Exception ex)
                {
                    _context.AddException($"向集合{CType.GetFriendlyName(_type)}中添加第[{List?.Count}]个元素失败,原因:{ex.Message}", ex);
                    return false;
                }
            }

            internal bool CreateInstance()
            {
                if (_type.IsInterface)
                {
                    List = new ArrayList();
                    return true;
                }
                try
                {
                    List = (IList)Activator.CreateInstance(_type);
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