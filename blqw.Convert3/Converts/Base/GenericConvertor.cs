using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Converts
{
    public abstract class GenericConvertor<T> : BaseConvertor<T>, IConvertor
    {
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected abstract IConvertor GetConvertor(Type outputType, Type[] genericTypes);

        IConvertor IConvertor.GetConvertor(Type outputType)
        {
            var parent = OutputType;
            var child = outputType;

            if (outputType == null)
                throw new ArgumentNullException(nameof(outputType));
            if (OutputType.IsGenericType == false)
                throw new ArgumentOutOfRangeException(nameof(outputType), "必须是泛型");

            Type[] genericTypes;
            if (IsCompatible(OutputType, outputType, out genericTypes))
            {
                var conv = GetConvertor(outputType, genericTypes);
                var type = typeof(InnerConvertor<>).MakeGenericType(conv.OutputType, outputType);
                return (IConvertor)Activator.CreateInstance(type, conv);
            }
            throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}无法兼容类型{OutputType}");
        }

        /// <summary>
        /// 判断一个类型兼容另一个类型
        /// </summary>
        /// <param name="definer">定义类型</param>
        /// <param name="tester">测试类型</param>
        /// <param name="compatibleGenericArgs">如果定义类完全兼容测试类,则返回兼容的泛型参数</param>
        /// <param name="testInherit">是否检测被测试类型的父类和接口</param>
        /// <returns></returns>
        private static bool IsCompatible(Type definer, Type tester, out Type[] compatibleGenericArgs, bool testInherit = true)
        {
            if (definer.IsAssignableFrom(tester))
            {
                compatibleGenericArgs = tester.GetGenericArguments();
                return true; //2个类本身存在继承关系
            }
            compatibleGenericArgs = Type.EmptyTypes;
            if (definer.IsGenericType == false)
            {
                return false; //否则如果definer不是泛型类,则不存在兼容的可能性
            }
            if (definer.IsGenericTypeDefinition == false)
            {
                definer = definer.GetGenericTypeDefinition(); //获取定义类型的泛型定义
            }
            if (tester.IsGenericType)
            {
                if (tester.IsGenericTypeDefinition)
                {
                    return false; //tester是泛型定义类型,无法兼容
                }
                //获取2个类的泛型参数
                var arg1 = ((TypeInfo)definer).GenericTypeParameters;
                var arg2 = tester.GetGenericArguments();
                //判断2个类型的泛型参数个数
                if (arg1.Length == arg2.Length)
                {
                    //判断definer 应用 tester泛型参数 后的继承关系
                    if (definer.MakeGenericType(arg2).IsAssignableFrom(tester))
                    {
                        compatibleGenericArgs = arg2;
                        return true;
                    }
                }
            }
            if (testInherit)
            {   //测试tester的父类是否被definer兼容
                var type = tester.BaseType;
                while (type != typeof(object))
                {
                    if (IsCompatible(definer, type, out compatibleGenericArgs, false))
                    {
                        return true;
                    }
                    type = type.BaseType;
                }
                //测试tester的接口是否被definer兼容
                foreach (var @interface in tester.GetInterfaces())
                {
                    if (IsCompatible(definer, @interface, out compatibleGenericArgs, false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        class InnerConvertor<TOutput> : GenericConvertor<TOutput>
            where TOutput : T
        {
            GenericConvertor<T> _convertor;
            public InnerConvertor(GenericConvertor<T> convertor)
            {
                _convertor = convertor;
            }
            protected override TOutput ChangeType(string input, Type outputType, out bool success)
            {
                return (TOutput)_convertor.ChangeType(input, outputType, out success);
            }

            protected override TOutput ChangeType(object input, Type outputType, out bool success)
            {
                return (TOutput)_convertor.ChangeType(input, outputType, out success);
            }

            protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
            {
                return _convertor.GetConvertor(outputType, genericTypes);
            }

            protected override void Initialize()
            {
                base.Initialize();
                _convertor.Initialize();
            }
        }

    }
}
