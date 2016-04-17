using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    [System.ComponentModel.Composition.Export(typeof(IConvertor))]
    public class CType : SystemTypeConvertor<Type>
    {
        static CType()
        {
            _keywords = new NameValueCollection();
            _keywords["bool"] = "System.Boolean";
            _keywords["byte"] = "System.Byte";
            _keywords["char"] = "System.Char";
            _keywords["decimal"] = "System.Decimal";
            _keywords["double"] = "System.Double";
            _keywords["short"] = "System.Int16";
            _keywords["int"] = "System.Int32";
            _keywords["long"] = "System.Int64";
            _keywords["sbyte"] = "System.SByte";
            _keywords["float"] = "System.Single";
            _keywords["string"] = "System.String";
            _keywords["object"] = "System.Object";
            _keywords["ushort"] = "System.UInt16";
            _keywords["uint"] = "System.UInt32";
            _keywords["ulong"] = "System.UInt64";
            _keywords["Guid"] = "System.Guid";
            
        }

        private static readonly NameValueCollection _keywords;
        private static readonly NameValueCollection _typeNames = new NameValueCollection();

        protected override bool Try(object input, out Type result)
        {
            result = input == null ? null : input.GetType();
            return true;
        }

        protected override bool Try(string input, out Type result)
        {
            result = Type.GetType(_keywords[input] ?? input, false, true) ?? Type.GetType("System." + input, false, true);

            ErrorContext.Error = new TypeLoadException(input + " 并不是一个类型");
            return result != null;
        }

        ///<summary> 获取类型名称的友好展现形式
        /// </summary>
        public static string GetDisplayName(Type t)
        {
            var s = _typeNames[t.GetHashCode().ToString()];
            if (s != null)
            {
                return s;
            }
            lock (_typeNames)
            {
                var t2 = Nullable.GetUnderlyingType(t);
                if (t2 != null)
                {
                    return _typeNames[t.GetHashCode().ToString()] = GetDisplayName(t2) + "?";

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
                        for (int i = 0; i < infos.Length; i++)
                        {
                            generic[i] = GetDisplayName(infos[i]);
                        }
                    }
                    return _typeNames[t.GetHashCode().ToString()] = GetSimpleName(t) + "<" + string.Join(", ", generic) + ">";
                }
                else
                {
                    return _typeNames[t.GetHashCode().ToString()] = GetSimpleName(t);
                }
            }
        }

        private static string GetSimpleName(Type t)
        {
            string name;
            switch (t.Namespace)
            {
                case "System":
                    switch (t.Name)
                    {
                        case "Boolean": return "bool";
                        case "Byte": return "byte";
                        case "Char": return "char";
                        case "Decimal": return "decimal";
                        case "Double": return "double";
                        case "Int16": return "short";
                        case "Int32": return "int";
                        case "Int64": return "long";
                        case "SByte": return "sbyte";
                        case "Single": return "float";
                        case "String": return "string";
                        case "Object": return "object";
                        case "UInt16": return "ushort";
                        case "UInt32": return "uint";
                        case "UInt64": return "ulong";
                        case "Guid": return "Guid";
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
                    name = t.Namespace + "." + t.Name;
                    break;
            }
            var index = name.LastIndexOf('`');
            if (index > -1)
            {
                name = name.Remove(index);
            }
            return name;
        }

        /// <summary> 获取TypeCode对应的Type对象
        /// </summary>
        /// <param name="typeCode">TypeCode</param>
        /// <returns></returns>
        public static Type GetType(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return typeof(Boolean);
                case TypeCode.Byte:
                    return typeof(Byte);
                case TypeCode.Char:
                    return typeof(Char);
                case TypeCode.DBNull:
                    return typeof(DBNull);
                case TypeCode.DateTime:
                    return typeof(DateTime);
                case TypeCode.Decimal:
                    return typeof(Decimal);
                case TypeCode.Double:
                    return typeof(Double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return typeof(Int16);
                case TypeCode.Int32:
                    return typeof(Int32);
                case TypeCode.Int64:
                    return typeof(Int64);
                case TypeCode.Object:
                    return typeof(Object);
                case TypeCode.SByte:
                    return typeof(SByte);
                case TypeCode.Single:
                    return typeof(Single);
                case TypeCode.String:
                    return typeof(String);
                case TypeCode.UInt16:
                    return typeof(UInt16);
                case TypeCode.UInt32:
                    return typeof(UInt32);
                case TypeCode.UInt64:
                    return typeof(UInt64);
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode));
            }
        }

        /// <summary> 获取接口类型数组中的获取指定类型的接口,如果不存在指定类型的接口则返回null
        /// </summary>
        /// <param name="interfaceTypes"> 接口类型集合 </param>
        /// <param name="interfaceType"> 需要判断比对的接口 </param>
        public static Type GetInterface(Type[] interfaceTypes, Type interfaceType)
        {
            if (interfaceType.IsGenericTypeDefinition)
            {
                foreach (var item in interfaceTypes)
                {
                    if (item.GetGenericTypeDefinition() == interfaceType)
                    {
                        return item;
                    }
                }
            }
            else if (interfaceType.IsGenericType)
            {
                var gt = interfaceType.GetGenericTypeDefinition();
                foreach (var item in interfaceTypes)
                {
                    if (item == interfaceType
                        || item.GetGenericTypeDefinition() == gt)
                    {
                        return item;
                    }
                }
            }
            else
            {
                foreach (var item in interfaceTypes)
                {
                    if (item == interfaceType)
                    {
                        return item;
                    }
                }
            }
            return null;
        }
    }
}
