using blqw.IOC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace blqw.Converts
{
    public class CIList : BaseTypeConvertor<IList>
    {
        protected override IList ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            if (input == null || input is DBNull)
            {
                return null;
            }

            var helper = new ListHelper(outputType);
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
                    Error.Add(new NotImplementedException("DataReader已经关闭"));
                    success = false;
                    return null;
                }
                while (reader.Read())
                {
                    var dict = (IDictionary<string, object>)new System.Dynamic.ExpandoObject();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dict[reader.GetName(i)] = reader.GetValue(i);
                    }
                    helper.Add(dict);
                }

                return helper.List;
            }

            var ee = (input as IEnumerable)?.GetEnumerator()
                    ?? (input as IEnumerator)
                    ?? (input as DataTable)?.Rows.GetEnumerator()
                    ?? (input as DataView)?.GetEnumerator()
                    ?? (input as DataRow)?.ItemArray.GetEnumerator()
                    ?? (input as DataRowView)?.Row.ItemArray.GetEnumerator()
                    ?? (input as IListSource)?.GetList()?.GetEnumerator();

            if (ee == null)
            {
                Error.CastFail("目前仅支持DataRow,DataRowView,或实现IEnumerator,IEnumerable,IListSource,IDataReader接口的对象转向IList");
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

        static readonly string[] Separator = { ", ", "," };
        protected override IList ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            input = input.Trim();
            if (input[0] == '[' && input[input.Length - 1] == ']')
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (IList)result;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                    success = false;
                    return null;
                }
            }
            var arr = input.Split(Separator, StringSplitOptions.None);
            return ChangeType(context, arr, outputType, out success);
        }

        struct ListHelper
        {
            public IList List;
            private Type _type;
            public ListHelper(Type type)
            {
                _type = type;
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
                    Error.Add(new NotSupportedException($"向集合{CType.GetFriendlyName(_type)}中添加第[{List?.Count}]个元素失败,原因:{ex.Message}", ex));
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
                    Error.Add(ex);
                    return false;
                }
            }
        }

    }
}
