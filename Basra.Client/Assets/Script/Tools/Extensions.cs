using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Basra.Client
{
    public static class Extensions
    {
        public static T Cast<T>(this object o)
        {
            return (T)o;
        }
        public static Type[] GetParameterTypes(this MethodInfo methodInfo)
        {
            var info = methodInfo.GetParameters();
            var types = new Type[info.Length];
            for (var i = 0; i < types.Length; i++)
            {
                types[i] = info[i].ParameterType;
            }
            return types;
        }
    }
}
