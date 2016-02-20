using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Dynamic
{
    public class DynamicDataRow : DynamicObject, IFormatProvider
    {

        object IFormatProvider.GetFormat(Type formatType)
        {
            if (formatType != null && string.Equals("Json", formatType.Name, StringComparison.Ordinal))
            {
                return _row;
            }
            return null;
        }

        DataRow _row;

        public DynamicDataRow(DataRow row)
        {
            _row = row;
        }

        public DynamicDataRow(DataRowView row)
        {
            _row = row.Row;
        }


        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (DataColumn col in _row.Table.Columns)
            {
                yield return col.ColumnName;
            }
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (typeof(IConvertible).IsAssignableFrom(binder.ReturnType))
            {
                var arr = _row.ItemArray;
                if (arr.Length == 1 && Convert3.TryChangedType(arr[0], binder.ReturnType, out result))
                {
                    return true;
                }
            }
            return Convert3.TryChangedType(_row, binder.ReturnType, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _row[binder.Name];
            if (result != null)
            {
                if (Convert3.TryChangedType(result, binder.ReturnType, out result))
                {
                    result = Convert3.ToDynamic(result);
                    return true;
                }
            }

            result = DynamicSystemObject.Null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (IsReadOnly)
            {
                return false;
                //throw new NotSupportedException("当前对象是只读的");
            }
            _row[binder.Name] = value.To<string>();
            return true;
        }


        private object Indexer(object[] indexes)
        {
            if (indexes == null || indexes.Length != 1)
            {
                return null;
            }
            var index = indexes[0];
            var name = index as string;
            if (name != null)
            {
                if (_row.Table.Columns.Contains(name))
                {
                    return _row[name];
                }
                else
                {
                    return null;
                }
            }
            var i = index.To<int>(-1);
            if (i < 0 || i > _row.ItemArray.Length)
            {
                return null;
            }
            return _row.ItemArray[i];
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Indexer(indexes);
            if (result != null)
            {
                if (Convert3.TryChangedType(result, binder.ReturnType, out result))
                {
                    result = result as DynamicObject ?? Convert3.ToDynamic(result);
                    return true;
                }
            }

            result = DynamicSystemObject.Null;
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (IsReadOnly)
            {
                return false;
                //throw new NotSupportedException("当前对象是只读的");
            }

            if (indexes == null || indexes.Length != 1)
            {
                return false;
            }
            var index = indexes[0];
            var name = index as string;
            if (name != null)
            {
                if (_row.Table.Columns.Contains(name))
                {
                    var col = _row.Table.Columns[name];
                    if (Convert3.TryChangedType(value, col.DataType, out value))
                    {
                        _row[col] = value;
                    }
                }
            }
            else
            {
                var i = index.To<int>(-1);
                if (i >= 0 && i < _row.ItemArray.Length)
                {
                    var col = _row.Table.Columns[i];
                    if (Convert3.TryChangedType(value, col.DataType, out value))
                    {
                        _row[col] = value;
                    }
                }

            }
            return false;
        }

        public bool IsReadOnly { get; set; }

    }
}
