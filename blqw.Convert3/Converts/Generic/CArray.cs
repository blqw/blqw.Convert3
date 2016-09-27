using System;
using System.Collections;
using blqw.IOC;

namespace blqw.Converts
{
    internal sealed class CArray<T> : BaseTypeConvertor<Array>
    {
        private static readonly string[] _Separator = { ", ", "," };

        public override Type OutputType => typeof(T[]);

        protected override Array ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            if ((input == null) || input is DBNull)
            {
                success = true;
                return null;
            }


            var ee = (input as IEnumerable)?.GetEnumerator()
                     ?? input as IEnumerator;
            if (ee == null)
            {
                success = false;
                return null;
            }
            var convertor = context.Get<T>();
            var array = new ArrayList();
            var elementType = convertor.OutputType;
            while (ee.MoveNext())
            {
                var value = convertor.ChangeType(context, ee.Current, elementType, out success);
                if (success == false)
                {
                    Error.Add(new ArrayTypeMismatchException($"{value?.ToString() ?? "<null>"} 写入数组失败!"));
                    success = false;
                    return null;
                }
                array.Add(value);
            }
            success = true;
            return array.ToArray(elementType);
        }

        protected override Array ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            input = input.Trim();
            if (input.Length == 0)
            {
                success = true;
                return Array.CreateInstance(outputType.GetElementType(), 0);
            }
            if ((input[0] == '[') && (input[input.Length - 1] == ']'))
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
                    success = true;
                    return (Array) result;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                    success = false;
                    return null;
                }
            }

            var convertor = context.Get<T>();
            if (convertor == null)
            {
                Error.ConvertorNotFound(typeof(T));
                success = false;
                return null;
            }
            var items = input.Split(_Separator, StringSplitOptions.None);
            var array = Array.CreateInstance(convertor.OutputType, items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                var value = convertor.ChangeType(context, items[i], convertor.OutputType, out success);
                if (success == false)
                {
                    Error.Add(new ArrayTypeMismatchException($"{value?.ToString() ?? "<null>"} 写入数组失败!"));
                    success = false;
                    return null;
                }
                array.SetValue(value, i);
            }
            success = true;
            return array;
        }
    }
}