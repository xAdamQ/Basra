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

        public static List<T> CutRange<T>(this List<T> from, int count, bool fromEnd = true)
        {
            int startIndex = fromEnd ? from.Count - count : 0;

            var part = from.GetRange(startIndex, count);
            from.RemoveRange(startIndex, count);

            return part;
        }

        public static T Cut<T>(this List<T> from, bool fromEnd = true)
        {
            var elementIndex = fromEnd ? from.Count - 1 : 0;

            var element = from[elementIndex];
            from.RemoveAt(elementIndex);

            return element;
        }

        public static T Cut<T>(this List<T> from, int index)
        {
            var element = from[index];
            from.RemoveAt(index);

            return element;
        }

    }
}
