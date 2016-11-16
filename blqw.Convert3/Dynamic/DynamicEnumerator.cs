using System;
using System.Collections;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace blqw.Dynamic
{
    internal class DynamicEnumerator : DynamicEntity, IObjectHandle, IObjectReference, ICustomTypeProvider, IEnumerator
    {
        private IEnumerator _enumerator;

        public DynamicEnumerator(IEnumerator enumerator)
            : base(enumerator)
        {
            _enumerator = enumerator;
        }

        public object Current => _enumerator?.ToDynamic();

        public Type GetCustomType() => _enumerator?.GetType();

        public object GetRealObject(StreamingContext context) => _enumerator;

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();

        public object Unwrap() => _enumerator;
    }
}