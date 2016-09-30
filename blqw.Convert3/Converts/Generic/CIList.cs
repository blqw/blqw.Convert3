using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using blqw.IOC;

namespace blqw.Converts
{
    internal sealed class CIList<T> : BaseTypeConvertor<ICollection<T>>
    {
        private static readonly string[] Separator = { ", ", "," };

        protected override ICollection<T> ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            success = true;
            if ((input == null) || input is DBNull)
            {
                return null;
            }
            var helper = new ListHelper(context, outputType);
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
                    if (helper.Add(reader) == false)
                    {
                        success = false;
                        return null;
                    }
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
                context.AddException("目前仅支持DataRow,DataRowView,或实现IEnumerator,IEnumerable,IListSource,IDataReader接口的对象转向IList");
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

        protected override ICollection<T> ChangeType(ConvertContext context, string input, Type outputType,
            out bool success)
        {
            input = input.Trim();
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
            var arr = input.Split(Separator, StringSplitOptions.None);
            return ChangeType(context, arr, outputType, out success);
        }

        private struct ListHelper
        {
            public ICollection<T> List;
            private readonly IConvertor<T> _convertor;
            private readonly ConvertContext _context;
            private readonly Type _type;

            public ListHelper(ConvertContext context, Type type)
            {
                _context = context;
                _type = type;
                List = null;
                _convertor = context.Get<T>();
            }

            public bool Add(object value)
            {
                bool b;
                var v = _convertor.ChangeType(_context, value, _convertor.OutputType, out b);
                if (b == false)
                {
                    _context.AddException($"向集合{CType.GetFriendlyName(_type)}中添加第[{List?.Count}]个元素失败");
                    return false;
                }
                try
                {
                    List.Add(v);
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
                    List = new List<T>();
                    return true;
                }
                try
                {
                    List = (ICollection<T>) Activator.CreateInstance(_type);
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