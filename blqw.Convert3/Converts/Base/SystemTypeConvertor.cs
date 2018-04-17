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

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected sealed override T ChangeType(ConvertContext context, object input, Type outputType, out bool success)
        {
            if (input.IsNull())
            {
                return ChangeTypeImpl(context, null, outputType, out success);
            }
            var row = input as DataRow;
            if (row != null)
            {
                switch (row.Table.Columns.Count)
                {
                    case 0:
                        return ChangeTypeImpl(context, null, outputType, out success);
                    case 1:
                        return BaseChangeType(context, row[0], outputType, out success);
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
                        return BaseChangeType(context, rv[0], outputType, out success);
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
                        return BaseChangeType(context, reader.GetValue(0), outputType, out success);
                    default:
                        success = false;
                        context.AddException("只有当 IDataReader 有且只有一列时才能尝试转换");
                        return default(T);
                }
            }

            var ee = (input as IEnumerable)?.GetEnumerator() ?? input as IEnumerator;
            if (ee?.MoveNext() == true)
            {
                var value = ee.Current;
                if (ee.MoveNext())
                {
                    success = false;
                    context.AddException("只有当 集合 有且只有一行时才能尝试转换");
                    return default(T);
                }
                return BaseChangeType(context, value, outputType, out success);
            }

            return ChangeTypeImpl(context, input, outputType, out success);
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected abstract T ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success);
    }
}