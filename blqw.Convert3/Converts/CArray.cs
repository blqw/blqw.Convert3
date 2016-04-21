using blqw.Convert3Component;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CArray<T> : AdvancedConvertor<Array>
    {
        protected override bool Try(object input, Type outputType, out Array result)
        {
            var emab = input as IEnumerable;
            var emtr = emab == null ? input as IEnumerator : emab.GetEnumerator();
            if (emtr == null)
            {
                result = null;
                return false;
            }
            var elementType = outputType.GetElementType();
            var conv = Convert3.GetConvertor(elementType);
            if (conv == null)
            {
                ErrorContext.ConvertorNotFound(elementType);
                result = null;
                return false;
            }

            var array = new ArrayList();
            while (emtr.MoveNext())
            {
                object value;
                if (conv.Try(emtr.Current, elementType, out value) == false)
                {
                    WriteFail(emtr.Current);
                    result = null;
                    return false;
                }
                array.Add(value);
            }
            result = array.ToArray(elementType);
            return true;
        }

        readonly static string[] Separator = { ", ", "," };

        protected override bool Try(string input, Type outputType, out Array result)
        {
            
        }

        static void WriteFail(object value)
        {
            var str = value == null ? "<null>" : value.ToString();
            ErrorContext.Error = new ArrayTypeMismatchException(value + " 写入数组失败!");
        }

        public override Array ChangeType(object input, Type outputType, out bool success)
        {
            throw new NotImplementedException();
        }

        public override Array ChangeType(string input, Type outputType, out bool success)
        {
            if (input.Length == 0)
            {
                success = true;
                return Array.CreateInstance(outputType.GetElementType(), 0);
            }
            input = input.Trim();
            if (input[0] == '[' && input[input.Length - 1] == ']')
            {
                try
                {
                    var result = Component.ToJsonObject(outputType, input);
                    success = true;
                    return (Array)result;
                }
                catch (Exception ex)
                {
                    Error.Add(ex);
                }
                success = false;
                return null;
            }
            var elementType = outputType.GetElementType();
            var conv = Convert3.GetConvertor(outputType.GetElementType());
            if (conv == null)
            {
                ErrorContext.ConvertorNotFound(elementType);
                result = null;
                return false;
            }
            var items = input.Split(Separator, StringSplitOptions.None);
            var array = Array.CreateInstance(elementType, items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                object value;
                if (conv.Try(items[i], elementType, out value) == false)
                {
                    WriteFail(items[i]);
                    result = null;
                    return false;
                }
                array.SetValue(value, i);
            }

            result = array;
            return true;
        }

        protected override IConvertor GetConvertor(Type outputType)
        {
            return base.GetConvertor(outputType);
        }
    }
}
