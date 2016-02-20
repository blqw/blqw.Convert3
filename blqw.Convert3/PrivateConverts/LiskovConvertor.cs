using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    sealed class LiskovConvertor<TBase, TOutput> : IConvertor<TOutput>, IIgnoreInherit
        where TOutput : TBase
    {
        static IConvertor<TBase> _conv;

        /// <summary> 转换器优先级,默认0
        /// </summary>
        public uint Priority { get { return 0; } }

        /// <summary> 转换器的输出类型
        /// (有可能是泛型定义类型)
        /// </summary>
        public Type OutputType { get; } = typeof(TOutput);
        
        void IConvertor.Initialize()
        {
            if (_conv == null)
            {
                _conv = Convert3.GetConvertor<TBase>();
            }
        }

        public bool Try(string input, Type outputType, out TOutput result)
        {
            if (_conv == null)
            {
                return CObject.TryTo(input, outputType, out result);
            }
            TBase r;
            if (_conv.Try(input, outputType, out r))
            {
                result = (TOutput)r;
                return true;
            }
            result = default(TOutput);
            return false;
        }

        public bool Try(object input, Type outputType, out TOutput result)
        {
            if (_conv == null)
            {
                return CObject.TryTo(input, outputType, out result);
            }
            TBase r;
            if (_conv.Try(input, outputType, out r))
            {
                result = (TOutput)r;
                return true;
            }
            result = default(TOutput);
            return false;
        }

        bool IConvertor.Try(string input, Type outputType, out object result)
        {
            if (_conv == null)
            {
                return CObject.TryTo(input, outputType, out result);
            }
            TBase r;
            if (_conv.Try(input, outputType, out r))
            {
                result = r;
                return true;
            }
            result = default(TOutput);
            return false;
        }

        bool IConvertor.Try(object input, Type outputType, out object result)
        {
            if (_conv == null)
            {
                return CObject.TryTo(input, outputType, out result);
            }
            TBase r;
            if (_conv.Try(input, outputType, out r))
            {
                result = r;
                return true;
            }
            result = default(TOutput);
            return false;
        }

    }
}
