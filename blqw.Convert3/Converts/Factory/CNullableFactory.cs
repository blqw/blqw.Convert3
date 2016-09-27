﻿using System;

namespace blqw.Converts
{
    internal sealed class CNullableFactory : GenericConvertor
    {
        public override Type OutputType => typeof(Nullable<>);

        /// <summary>
        /// 根据返回类型的泛型参数类型返回新的转换器
        /// </summary>
        /// <param name="outputType"> </param>
        /// <param name="genericTypes"> </param>
        /// <returns> </returns>
        protected override IConvertor GetConvertor(Type outputType, Type[] genericTypes)
        {
            var type = typeof(CNullable<>).MakeGenericType(genericTypes);
            return (IConvertor) Activator.CreateInstance(type);
        }
    }
}