using System;
using System.Collections.Specialized;

namespace blqw.Converts
{
    internal sealed class CType : SystemTypeConvertor<Type>
    {
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

        private static readonly StringDictionary _TypeNames = new StringDictionary();

        protected override Type ChangeTypeImpl(ConvertContext context, object input, Type outputType, out bool success)
        {
            success = true;
            return input?.GetType();
        }

        protected override Type ChangeType(ConvertContext context, string input, Type outputType, out bool success)
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
        public static string GetFriendlyName(Type t)
        {
            if (t == null)
            {
                return "`null`";
            }
            var s = _TypeNames[t.GetHashCode().ToString()];
            if (s != null)
            {
                return s;
            }
            lock (_TypeNames)
            {
                var t2 = Nullable.GetUnderlyingType(t);
                if (t2 != null)
                {
                    return _TypeNames[t.GetHashCode().ToString()] = GetFriendlyName(t2) + "?";
                }
                if (t.IsGenericType)
                {
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
                    return
                        _TypeNames[t.GetHashCode().ToString()] =
                            GetSimpleName(t) + "<" + string.Join(", ", generic) + ">";
                }
                return _TypeNames[t.GetHashCode().ToString()] = GetSimpleName(t);
            }
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
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="typeCode"/> 错误.</exception>
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