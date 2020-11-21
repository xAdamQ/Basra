using System;
using System.Collections.Generic;

namespace Basra.Server.Extensions
{
    public static class General
    {
        public static void Remove<T>(this List<T> list, Predicate<T> predicate)
        {
            list.RemoveAt(list.FindIndex(predicate));
        }

        public static void Shuffle<T>(this IList<T> List)
        {
            var random = new Random();
            for (int i = 0; i < List.Count; i++)
            {
                var temp = List[i];
                int randomIndex = random.Next(i, List.Count);
                List[i] = List[randomIndex];
                List[randomIndex] = temp;
            }
        }

        public static List<T> CutRange<T>(this List<T> from, int count, bool fromEnd = true)
        {
            var part = from.GetRange(from.Count - count, count);
            from.RemoveRange(from.Count - count, count);

            return part;
        }
    }
}