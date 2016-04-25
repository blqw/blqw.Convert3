using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CIListT: CIList<object>
    {
        public override Type OutputType
        {
            get
            {
                return typeof(ICollection<>);
            }
        }
    }
    public class CIList<T> : GenericConvertor<ICollection<T>>
    {
        IConvertor<T> _convertor;
        protected override void Initialize()
        {
            _convertor = ConvertorContainer.Default.Get<T>();
        }
        protected override ICollection<T> ChangeType(object input, Type outputType, out bool success)
        {
            success = true;
            if (input == null || input is DBNull)
            {
                return null;
            }

            var helper = new ListHelper(outputType, _convertor);
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
                    if (helper.Add(reader) == false)
                    {
                        success = false;
                        return null;
                    }
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
        protected override ICollection<T> ChangeType(string input, Type outputType, out bool success)
        {
            input = input.Trim();
            if (input[0] == '[' && input[input.Length - 1] == ']')
            {
                try
                {
                    var result = Convert3Component.Component.ToJsonObject(outputType, input);
                    success = true;
                    return (ICollection<T>)result;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                    success = false;
                    return null;
                }
            }
            var arr = input.Split(Separator, StringSplitOptions.None);
            return ChangeType(arr, outputType, out success);
        }

        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CIList<>).MakeGenericType(genericTypes);
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }

        struct ListHelper
        {
            public ICollection<T> List;
            private IConvertor<T> _convertor;
            private Type _type;

            public ListHelper(Type type, IConvertor<T> convertor) 
            {
                _type = type;
                List = null;
                this._convertor = convertor;
            }

            public bool Add(object value)
            {
                bool b;
                var v = _convertor.ChangeType(value, _convertor.OutputType, out b);
                if (b == false)
                {
                    return false;
                }
                try
                {
                    List.Add(v);
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
                    List = (ICollection<T>)Activator.CreateInstance(_type);
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
