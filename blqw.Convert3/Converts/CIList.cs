using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace blqw.Converts
{
    public class CIList : AdvancedConvertor<IList>
    {
        protected override IList ChangeType(object input, Type outputType, out bool success)
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
                Error.CastFail("目前仅支持DataRow,DataRowView,或实现IEnumerator,IEnumerable,IListSource接口的对象转向IList");
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

        readonly static string[] Separator = { ", ", "," };
        protected override IList ChangeType(string input, Type outputType, out bool success)
        {
            input = input.Trim();
            if (input[0] == '[' && input[input.Length - 1] == ']')
            {
                try
                {
                    var result = Convert3Component.Component.ToJsonObject(outputType, input);
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
            success = true;
            return input.Split(Separator, StringSplitOptions.None);
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
                    Error.Add(ex);
                    return false;
                }
            }

            internal bool CreateInstance()
            {
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
