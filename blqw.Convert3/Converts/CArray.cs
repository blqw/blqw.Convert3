using blqw.IOC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public class CArray : CArray<object>
    {
        public override Type OutputType
        {
            get
            {
                return typeof(Array);
            }
        }
    }

    public class CArray<T> : AdvancedConvertor<Array>
    {
        readonly static string[] Separator = { ", ", "," };

        private IConvertor<T> _ElementConvertor;

        protected override void Initialize()
        {
            base.Initialize();
            _ElementConvertor = ConvertorServices.Container.GetConvertor<T>();
        }

        public override Type OutputType
        {
            get
            {
                return typeof(T[]);
            }
        }

        protected override Array ChangeType(object input, Type outputType, out bool success)
        {
            if (input == null || input is DBNull)
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
            if (input.Length == 0)
            {
                success = true;
                return Array.CreateInstance(outputType.GetElementType(), 0);
            }
            if (input[0] == '[' && input[input.Length - 1] == ']')
            {
                try
                {
                    var result = ComponentServices.ToJsonObject(outputType, input);
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

        protected override IConvertor GetConvertor(Type outputType)
        {
            var type = typeof(CArray<>).MakeGenericType(outputType.GetElementType());
            var conv = (IConvertor)Activator.CreateInstance(type);
            return conv;
        }
    }
}
