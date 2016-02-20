
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw;
using blqw.Convert3Component;

/// <summary> 用于存放系统拓展方法
/// </summary>
static class ExtensionMethods
{

    public static string DisplayName(this Type type)
    {
        return CType.GetDisplayName(type);
    }
}

