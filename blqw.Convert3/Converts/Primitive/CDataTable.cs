using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using blqw.IOC;

namespace blqw.Converts
{
    internal sealed class CDataTable : BaseTypeConvertor<DataTable>
    {
        protected override DataTable ChangeTypeImpl(ConvertContext context, object input, Type outputType,
            out bool success)
        {
            success = true;
            var view = input as DataView;
            if (view != null)
            {
                return view.ToTable();
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
                var table1 = new DataTable();
                table1.Load(reader);
                return table1;
            }

            var ee = (input as IEnumerable)?.GetEnumerator()
                     ?? input as IEnumerator
                     ?? (input as DataTable)?.Rows.GetEnumerator()
                     ?? (input as DataRow)?.ItemArray.GetEnumerator()
                     ?? (input as DataRowView)?.Row.ItemArray.GetEnumerator()
                     ?? (input as IListSource)?.GetList()?.GetEnumerator();

            if (ee == null)
            {
                context.AddException("仅支持DataView,DataRow,DataRowView,或实现IEnumerator,IEnumerable,IListSource,IDataReader接口的对象对DataTable的转换");
                success = false;
                return null;
            }
            var table = new DataTable();
            var helper = new DataRowHelper(table,context);
            while (ee.MoveNext())
            {
                if (helper.CreateRow(ee.Current) == false)
                {
                    success = false;
                    return null;
                }
                helper.AddRow();
            }
            return table;
        }

        protected override DataTable ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            input = input.Trim();
            if ((input[0] == '{') && (input[input.Length - 1] == '}'))
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (DataTable) result;
                }
                catch (Exception ex)
                {
                    context.AddException(ex);
                    success = false;
                    return null;
                }
            }
            success = false;
            return null;
        }


        private struct DataRowHelper
        {
            private readonly DataTable _table;
            private readonly ConvertContext _context;
            private readonly DataColumnCollection _columns;

            public DataRowHelper(DataTable table, ConvertContext context)
            {
                _table = table;
                _context = context;
                _columns = _table.Columns;
                _currentRow = null;
            }

            private DataRow _currentRow;

            public void AddRow()
            {
                _table.Rows.Add(_currentRow);
            }

            public bool CreateRow(object value)
            {
                _currentRow = _table.NewRow();
                var nv = value as NameValueCollection;
                if (nv != null)
                {
                    foreach (string name in nv)
                    {
                        if (AddCell(name, typeof(string), nv[name]) == false)
                        {
                            return false;
                        }
                    }
                    return true;
                }

                var ee = (value as IEnumerable)?.GetEnumerator() ?? value as IEnumerator;
                if (ee != null)
                {
                    const BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
                    while (ee.MoveNext())
                    {
                        var entry = ee.Current;
                        if (entry == null)
                        {
                            continue;
                        }
                        var type = entry.GetType();
                        var getKey = type.GetProperty("Key", flags).GetPropertyHandler()?.Get ??
                                     type.GetProperty("Name", flags).GetPropertyHandler()?.Get;
                        var getValue = type.GetProperty("Value", flags).GetPropertyHandler()?.Get;
                        if ((getKey == null) || (getValue == null))
                        {
                            _context.AddException("值添加到单元格失败:无法获取Key/Name和Value");
                            return false;
                        }
                        do
                        {
                            entry = ee.Current;
                            var name = getKey(entry) as string;
                            if (name == null)
                            {
                                _context.AddException("标题必须为字符串");
                                return false;
                            }
                            if (AddCell(name, typeof(object), getValue(entry)) == false)
                            {
                                return false;
                            }
                        } while (ee.MoveNext());
                        return true;
                    }
                    return true;
                }

                {
                    //实体对象
                    var props = PublicPropertyCache.GetByType(value.GetType());
                    for (int i = 0, length = props.Length; i < length; i++)
                    {
                        var p = props[i];
                        if (AddCell(p.Name, p.Property.PropertyType, p.Get(value)) == false)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }


            private bool AddCell(string name, Type type, object value)
            {
                var col = _columns[name];
                if (col == null)
                {
                    col = _columns.Add(name, type);
                }
                else if (col.DataType != type)
                {
                    bool success;
                    value = value.ChangeType(col.DataType, out success);
                    if (success == false)
                    {
                        _context.AddException($"第{_table?.Rows.Count}行{col.ColumnName}列添加到行失败");
                        return false;
                    }
                }
                _currentRow[col] = value;
                return true;
            }
        }
    }
}