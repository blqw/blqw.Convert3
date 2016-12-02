using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using blqw.IOC;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="DataTable" /> 转换器
    /// </summary>
    public class CDataTable : BaseTypeConvertor<DataTable>
    {
        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
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
            var builder = new DataTableBuilder(context);
            builder.TryCreateInstance();
            while (ee.MoveNext())
            {
                if (builder.Set(ee.Current) == false)
                {
                    success = false;
                    return null;
                }
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

        /// <summary>
        /// <seealso cref="DataTable" /> 构造器
        /// </summary>
        protected struct DataTableBuilder : IBuilder<DataTable, object>
        {
            /// <summary>
            /// 转换上下文
            /// </summary>
            private readonly ConvertContext _context;

            /// <summary>
            ///     <seealso cref="DataTable.Columns" />
            /// </summary>
            private DataColumnCollection _columns;

            /// <summary>
            /// 初始化构造器
            /// </summary>
            /// <param name="context"> 转换上下文 </param>
            public DataTableBuilder(ConvertContext context)
            {
                _context = context;
                _columns = null;
                Instance = null;
            }

            /// <summary>
            /// 设置对象值
            /// </summary>
            /// <param name="value"> 待设置的值 </param>
            /// <returns> </returns>
            public bool Set(object value)
            {
                var mapper = new Mapper(value);
                if (mapper.Error!=null)
                {
                    _context.AddException(mapper.Error);
                    return false;
                }
                var row = Instance.NewRow();

                while (mapper.MoveNext())
                {
                    var name = mapper.Key as string;
                    if (name == null)
                    {
                        _context.AddException("标题必须为字符串");
                        return false;
                    }
                    if (AddCell(row, name, typeof(object), mapper.Value) == false)
                    {
                        return false;
                    }
                }
                Instance.Rows.Add(row);
                return true;
            }

            /// <summary>
            /// 添加单元格中的值
            /// </summary>
            /// <param name="row"> 需要添加单元格的行 </param>
            /// <param name="name"> 单元格列名 </param>
            /// <param name="type"> 值类型 </param>
            /// <param name="value"> 需要添加的值 </param>
            /// <returns> </returns>
            private bool AddCell(DataRow row, string name, Type type, object value)
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
                        _context.AddException($"第{Instance?.Rows.Count}行{col.ColumnName}列添加到行失败");
                        return false;
                    }
                }
                row[col] = value;
                return true;
            }

            /// <summary>
            /// 被构造的实例
            /// </summary>
            public DataTable Instance { get; private set; }

            /// <summary>
            /// 尝试构造实例,返回是否成功
            /// </summary>
            /// <returns> </returns>
            public bool TryCreateInstance()
            {
                Instance = new DataTable();
                _columns = Instance.Columns;
                return true;
            }
        }
    }
}