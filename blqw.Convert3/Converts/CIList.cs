using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CIList : AdvancedConvertor<IList>
    {
        struct DataReaderEnumerable : IEnumerator
        {
            IDataReader _reader;
            public DataReaderEnumerable(IDataReader reader)
            {
                _reader = reader;
            }
            public object Current
            {
                get { return _reader; }
            }

            public bool MoveNext()
            {
                return _reader.Read();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
        private static IEnumerator GetIEnumerator(object input)
        {
            var row = input as DataRow;
            if (row != null)
            {
                return row.ItemArray.GetEnumerator(); ;
            }
            var rv = input as DataRowView;
            if (rv != null)
            {
                return rv.Row.ItemArray.GetEnumerator(); ;
            }
            var reader = input as IDataReader;
            if (reader != null)
            {
                if (reader.IsClosed)
                {
                    ErrorContext.Error = new NotImplementedException("DataReader已经关闭");
                    return null;
                }
                return new DataReaderEnumerable(reader);
            }
            var ls = input as IListSource;
            if (ls != null)
            {
                return ls.GetList().GetEnumerator();
            }
            var emtr = input as IEnumerator;
            if (emtr != null)
            {
                return emtr;
            }
            var emab = input as IEnumerable;
            if (emab != null)
            {
                return emab.GetEnumerator();
            }
            return null;
        }

        protected override bool Try(object input, Type outputType, out IList result)
        {
            var emtr = GetIEnumerator(input);
            if (emtr == null)
            {
                if (input is IDataReader)
                {
                    result = null;
                    return false;
                }
                ErrorContext.Error = new InvalidCastException("目前仅支持DataRow,DataRowView,或实现IEnumerator,IEnumerable,IListSource接口的对象转向IList");
                result = null;
                return false;
            }

            Type elementType;
            IConvertor conv;
            if (outputType.IsGenericType)
            {
                if (outputType.GenericTypeArguments.Length != 1)
                {
                    ErrorContext.Error = new InvalidCastException("无法推断IList的元素类型");
                    result = null;
                    return false;
                }
                elementType = outputType.GenericTypeArguments[0];
                conv = Convert3.GetConvertor(elementType);
                if (conv == null)
                {
                    ErrorContext.ConvertorNotFound(elementType);
                    result = null;
                    return false;
                }
            }
            else
            {
                elementType = typeof(object);
                conv = Convert3.GetConvertor<object>();
            }

            if (outputType.IsInterface)
            {
                outputType = typeof(List<>).MakeGenericType(elementType);
            }

            var list = (IList)Activator.CreateInstance(outputType);
            while (emtr.MoveNext())
            {
                object value;
                if (conv.Try(emtr.Current, elementType, out value) == false)
                {
                    ErrorContext.Error = new NotImplementedException("IList元素值转换失败");
                    result = null;
                    return false;
                }
                list.Add(value);
            }
            result = list;
            return true;
        }

        readonly static string[] Separator = { ", ", "," };

        protected override bool Try(string input, Type outputType, out IList result)
        {
            input = input.Trim();
            if (input[0] == '[' && input[input.Length - 1] == ']')
            {
                return CJsonObject.TryTo(input, outputType, out result);
            }
            var arr = input.Split(Separator, StringSplitOptions.None);
            return Try(arr, outputType, out result);
        }
    }
}
