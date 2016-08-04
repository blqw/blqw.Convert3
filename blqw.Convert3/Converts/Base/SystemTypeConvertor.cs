using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    /// <summary> 
    /// 系统类型转换器,额外处理 DataRow,DataRowView,DataReader
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SystemTypeConvertor<T> : BaseConvertor<T>
    {
        protected override bool TryConvertString { get; } = true;

        protected override T ChangeTypeImpl(object input, Type outputType, out bool success)
        {
            var row = input as DataRow;
            if (row != null)
            {
                var arr = row.ItemArray;
                if (arr.Length > 0)
                {
                    return This.ChangeType(arr[0], outputType, out success);
                }
                return This.ChangeType(null, outputType, out success);
            }
            var rv = input as DataRowView;
            if (rv != null)
            {
                if (rv.DataView.Table.Columns.Count > 0)
                {
                    return This.ChangeType(rv[0], outputType, out success);
                }
                return This.ChangeType(null, outputType, out success);
            }
            var reader = input as IDataReader;
            if (reader != null)
            {
                if (reader.FieldCount > 0)
                {
                    return This.ChangeType(reader.GetValue(0), outputType, out success);
                }
                return This.ChangeType(null, outputType, out success);
            }

            return ChangeType(input, outputType, out success);
        }

    }
}
