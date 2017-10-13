using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace blqw.Converts
{
    /// <summary>
    /// <seealso cref="Type" /> 转换器
    /// </summary>
    public class CType : SystemTypeConvertor<Type>
    {
        /// <summary>
        /// 系统关键字映射
        /// </summary>
        private static readonly StringDictionary _Keywords = new StringDictionary
        {
            ["bool"] = "System.Boolean",
            ["byte"] = "System.Byte",
            ["char"] = "System.Char",
            ["decimal"] = "System.Decimal",
            ["double"] = "System.Double",
            ["short"] = "System.Int16",
            ["int"] = "System.Int32",
            ["long"] = "System.Int64",
            ["sbyte"] = "System.SByte",
            ["float"] = "System.Single",
            ["string"] = "System.String",
            ["object"] = "System.Object",
            ["ushort"] = "System.UInt16",
            ["uint"] = "System.UInt32",
            ["ulong"] = "System.UInt64",
            ["Guid"] = "System.Guid"
        };

        /// <summary>
        /// 类型名称缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type,string> _TypeNames = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override Type ChangeTypeImpl(IConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            return input?.GetType();
        }

        /// <summary>
        /// 返回指定类型的对象，其值等效于指定字符串对象。
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="input"> 需要转换类型的字符串对象 </param>
        /// <param name="outputType"> 换转后的类型 </param>
        /// <param name="success"> 是否成功 </param>
        protected override Type ChangeType(IConvertContext context, string input, Type outputType, out bool success)
        {
            if (input == null)
            {
                success = true;
                return null;
            }
            var result = Type.GetType(_Keywords[input] ?? input, false, true) ??
                         Type.GetType("System." + input, false, true);
            if (result == null)
            {
                context.AddException(new TypeLoadException(input + " 并不是一个类型"));
                success = false;
            }
            else
            {
                success = true;
            }
            return result;
        }

        /// <summary>
        /// 获取类型名称的友好展现形式
        /// </summary>
        public static string GetFriendlyName(Type type)
        {
            if (type == null)
            {
                return "`null`";
            }

            return _TypeNames.GetOrAdd(type, t =>
            {
                var t2 = Nullable.GetUnderlyingType(t);
                if (t2 != null)
                {
                    return GetFriendlyName(t2) + "?";
                }
                if (!t.IsGenericType)
                {
                    return GetSimpleName(t);
                }
                string[] generic;
                if (t.IsGenericTypeDefinition) //泛型定义
                {
                    var args = t.GetGenericArguments();
                    generic = new string[args.Length];
                }
                else
                {
                    var infos = t.GetGenericArguments();
                    generic = new string[infos.Length];
                    for (var i = 0; i < infos.Length; i++)
                    {
                        generic[i] = GetFriendlyName(infos[i]);
                    }
                }
                return GetSimpleName(t) + "<" + string.Join(", ", generic) + ">";
            });
        }

        private static string GetSimpleName(Type t)
        {
            string name;
            if (t.ReflectedType == null)
            {
                switch (t.Namespace)
                {
                    case "System":
                        switch (t.Name)
                        {
                            case "Boolean":
                                return "bool";
                            case "Byte":
                                return "byte";
                            case "Char":
                                return "char";
                            case "Decimal":
                                return "decimal";
                            case "Double":
                                return "double";
                            case "Int16":
                                return "short";
                            case "Int32":
                                return "int";
                            case "Int64":
                                return "long";
                            case "SByte":
                                return "sbyte";
                            case "Single":
                                return "float";
                            case "String":
                                return "string";
                            case "Object":
                                return "object";
                            case "UInt16":
                                return "ushort";
                            case "UInt32":
                                return "uint";
                            case "UInt64":
                                return "ulong";
                            case "Guid":
                                return "Guid";
                            default:
                                name = t.Name;
                                break;
                        }
                        break;
                    case null:
                    case "System.Collections":
                    case "System.Collections.Generic":
                    case "System.Data":
                    case "System.Text":
                    case "System.IO":
                    case "System.Collections.Specialized":
                        name = t.Name;
                        break;
                    default:
                        name = $"{t.Namespace}.{t.Name}";
                        break;
                }
            }
            else
            {
                name = $"{GetSimpleName(t.ReflectedType)}.{t.Name}";
            }
            var index = name.LastIndexOf('`');
            if (index > -1)
            {
                name = name.Remove(index);
            }
            return name;
        }

        /// <summary>
        /// 获取TypeCode对应的Type对象
        /// </summary>
        /// <param name="typeCode"> TypeCode </param>
        /// <returns> </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="typeCode" /> 错误. </exception>
        public static Type GetType(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return typeof(bool);
                case TypeCode.Byte:
                    return typeof(byte);
                case TypeCode.Char:
                    return typeof(char);
                case TypeCode.DBNull:
                    return typeof(DBNull);
                case TypeCode.DateTime:
                    return typeof(DateTime);
                case TypeCode.Decimal:
                    return typeof(decimal);
                case TypeCode.Double:
                    return typeof(double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return typeof(short);
                case TypeCode.Int32:
                    return typeof(int);
                case TypeCode.Int64:
                    return typeof(long);
                case TypeCode.Object:
                    return typeof(object);
                case TypeCode.SByte:
                    return typeof(sbyte);
                case TypeCode.Single:
                    return typeof(float);
                case TypeCode.String:
                    return typeof(string);
                case TypeCode.UInt16:
                    return typeof(ushort);
                case TypeCode.UInt32:
                    return typeof(uint);
                case TypeCode.UInt64:
                    return typeof(ulong);
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode));
            }
        }
    }
}