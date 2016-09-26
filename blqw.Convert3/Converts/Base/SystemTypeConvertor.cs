using System;
using System.Collections;
using System.Data;

namespace blqw.Converts
{
    /// <summary>
    /// 系统类型转换器,额外处理 DataRow,DataRowView,DataReader
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public abstract class SystemTypeConvertor<T> : BaseConvertor<T>
    {
        /// <summary>
        /// 返回是否应该尝试转换String后再转换
        /// </summary>
        protected override bool ShouldConvertString => true;

        protected sealed override T ChangeType(ConvertContext context, object input, Type outputType, out bool success)
        {
            var row = input as DataRow;
            if (row != null)
            {
                switch (row.Table.Columns.Count)
                {
                    case 0:
                        return ChangeTypeImpl(context, null, outputType, out success);
                    case 1:
                        return ChangeTypeImpl(context, row[0], outputType, out success);
                    default:
                        success = false;
                        context.AddException("只有当 DataRow 有且只有一列时才能尝试转换");
                        return default(T);
                }
            }

            var rv = input as DataRowView;
            if (rv != null)
            {
                switch (rv.DataView.Table.Columns.Count)
                {
                    case 0:
                        return ChangeTypeImpl(context, null, outputType, out success);
                    case 1:
                        return ChangeTypeImpl(context, rv[0], outputType, out success);
                    default:
                        success = false;
                        context.AddException("只有当 DataRowView 有且只有一列时才能尝试转换");
                        return default(T);
                }
            }

            var reader = input as IDataReader;
            if (reader != null)
            {
                switch (reader.FieldCount)
                {
                    case 0:
                        return ChangeTypeImpl(context, null, outputType, out success);
                    case 1:
                        return ChangeTypeImpl(context, reader.GetValue(0), outputType, out success);
                    default:
                        success = false;
                        context.AddException("只有当 IDataReader 有且只有一列时才能尝试转换");
                        return default(T);
                }
            }

            var ee = (input as IEnumerable)?.GetEnumerator() ?? input as IEnumerator;
            if (ee.MoveNext())
            {
                var value = ee.Current;
                if (ee.MoveNext())
                {
                    success = false;
                    context.AddException("只有当 集合 有且只有一行时才能尝试转换");
                    return default(T);
                }
                return ChangeTypeImpl(context, value, outputType, out success);
            }

            return ChangeTypeImpl(context, input, outputType, out success);
        }

        protected abstract T ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success);
    }
}