using blqw.Convert3Component;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public class CArray : AdvancedConvertor<Array>
    {
        protected override Array ChangeType(string input, Type outputType, out bool success)
        {
            throw new NotImplementedException();
        }

        protected override Array ChangeType(object input, Type outputType, out bool success)
        {
            throw new NotImplementedException();
        }

        protected override IConvertor GetConvertor(Type outputType)
        {
            var type = typeof(CArray<>).MakeGenericType(outputType);
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }
    }

    public class CArray<T> : CArray
    {
        readonly static string[] Separator = { ", ", "," };
        
        private IConvertor<T> _ElementConvertor;

        protected override void Initialize()
        {
            base.Initialize();
            _ElementConvertor = ConvertorContainer.Default.Get<T>();
        }

        protected override Array ChangeType(object input, Type outputType, out bool success)
        {
            if (input == null)
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
            var conv = _ElementConvertor;
            if (conv == null)
            {
                Error.ConvertorNotFound(typeof(T));
                success = false;
                return null;
            }

            var array = new ArrayList();
            var elementType = _ElementConvertor.OutputType;
            while (ee.MoveNext())
            {
                var value = conv.ChangeType(ee.Current, elementType, out success);
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

        protected override Array ChangeType(string input, Type outputType, out bool success)
        {
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
                    success = false;
                    return null;
                }
            }

            var conv = _ElementConvertor;
            if (conv == null)
            {
                Error.ConvertorNotFound(typeof(T));
                success = false;
                return null;
            }
            var items = input.Split(Separator, StringSplitOptions.None);
            var array = Array.CreateInstance(conv.OutputType, items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                var value = conv.ChangeType(items[i], conv.OutputType, out success);
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
