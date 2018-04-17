using System;
using System.Collections;
using blqw.IOC;

namespace blqw.Converts
{
    /// <summary>
    /// 数组转换器
    /// </summary>
    /// <typeparam name="T"> 数组元素类型 </typeparam>
    public class CArray<T> : BaseTypeConvertor<T[]>
    {
        /// <summary>
        /// 用于在字符串中拆分数组元素的分隔符
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly string[] _Separator = { ", ", "," };

        /// <summary>
        /// 转换器的输出类型
        /// </summary>
        public override Type OutputType => typeof(T);


        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        /// <returns> </returns>
        protected override T[] ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            if (input.IsNull())
            {
                success = true;
                return null;
            }

            var convertor = context.Get<T>();
            var array = new ArrayList();
            var elementType = convertor.OutputType;

            //获取对象的枚举器
            var ee = (input as IEnumerable)?.GetEnumerator()
                     ?? input as IEnumerator;
            if (ee == null)
            {
                var value = convertor.ChangeType(context, input, elementType, out success);
                if (success == false)
                {
                    success = false;
                    return null;
                }
                array.Add(value);
                success = true;
                return (T[])array.ToArray(elementType); //输出数组
            }

            while (ee.MoveNext()) //循环转换枚举器中的对象,并构造数组
            {
                var value = convertor.ChangeType(context, ee.Current, elementType, out success);
                if (success == false)
                {
                    context.AddException($"{value?.ToString() ?? "<null>"} 写入数组失败!");
                    return null;
                }
                array.Add(value);
            }
            success = true;
            return (T[]) array.ToArray(elementType); //输出数组
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override T[] ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            input = input?.Trim() ?? "";
            if (input.Length == 0)
            {
                success = true;
                return (T[]) Array.CreateInstance(outputType.GetElementType(), 0);
            }
            if ((input[0] == '[') && (input[input.Length - 1] == ']')) //判断如果是json字符串,使用json方式转换
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (T[]) result;
                }
                catch (Exception ex)
                {
                    context.AddException(ex);
                    success = false;
                    return null;
                }
            }

            var convertor = context.Get<T>();
            var items = input.Split(_Separator, StringSplitOptions.None);
            var array = Array.CreateInstance(convertor.OutputType, items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                var value = convertor.ChangeType(context, items[i], convertor.OutputType, out success);
                if (success == false)
                {
                    context.AddException($"{value?.ToString() ?? "<null>"} 写入数组失败!");
                    return null;
                }
                array.SetValue(value, i);
            }
            success = true;
            return (T[]) array;
        }
    }
}