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
        public GenericConvertor()
        {
            if (OutputType.IsGenericType == false)
            {
                throw new ArgumentOutOfRangeException(nameof(OutputType), "必须是泛型");
            }
        }

        protected abstract IConvertor GetConvertor(Type outputType, Type[] types);

        IConvertor IConvertor.GetConvertor(Type outputType)
        {
            var parent = OutputType;
            var child = outputType;

            if (outputType == null)
                throw new ArgumentNullException(nameof(outputType));
            if (OutputType.IsGenericType == false)
                throw new ArgumentOutOfRangeException(nameof(outputType), "必须是泛型");

            //if (CanCast(OutputType, outputType) == false)
                throw new ArgumentOutOfRangeException(nameof(outputType), $"类型{outputType}不是{OutputType}的子类或实现类");


        }


        private static bool CanCast(Type parent, Type child, bool checkInherit, out Type[] gengericArgument)
        {
            gengericArgument = null;
            if (parent.IsAssignableFrom(child))
            {
                return true;
            }

            if (parent.IsGenericType && child.IsGenericType)
            {
                if (child.IsGenericTypeDefinition)
                {
                    return false;
                }


                var arg1 = parent.IsGenericTypeDefinition ?
                            ((TypeInfo)parent).GenericTypeParameters :
                            parent.GetGenericArguments();
                var arg2 = child.IsGenericTypeDefinition ?
                            ((TypeInfo)child).GenericTypeParameters :
                            child.GetGenericArguments();

                if (arg1.Length != arg2.Length)
                {
                    return false;
                }
                var gtd1 = parent.IsGenericTypeDefinition ? parent : parent.GetGenericTypeDefinition();
                if (gtd1.MakeGenericType(arg2).IsAssignableFrom(child) == false)
                {
                    return false;
                }
                if (parent.IsGenericTypeDefinition)
                {
                    gengericArgument = arg2;
                    return true;
                }
                for (int i = 0; i < arg1.Length; i++)
                {
                    if (CanCast(arg1[i], arg2[i], true, out gengericArgument) == false)
                    {
                        return false;
                    }
                }
                gengericArgument = arg2;
                return true;
            }
            if (checkInherit)
            {
                var type = child.BaseType;
                while (type != typeof(object))
                {
                    if (CanCast(parent, type, false, out gengericArgument))
                    {
                        return true;
                    }
                }

                foreach (var @interface in child.GetInterfaces())
                {
                    if (CanCast(parent, @interface, false, out gengericArgument))
                    {
                        return true;
                    }
                }
            }



            return false;
        }









    }
}
