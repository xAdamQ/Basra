using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public static IEnumerable<T[]> Permutations<T>(this IEnumerable<T> source)
        {
            if (null == source)
                throw new ArgumentNullException(nameof(source));

            T[] data = source.ToArray();

            return Enumerable
              .Range(0, 1 << (data.Length))
              .Select(index => data
                 .Where((v, i) => (index & (1 << i)) != 0)
                 .ToArray());
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

        public static bool InRange(this int value, int max, int min = 0)
        {
            return (value < max && value >= min);
        }
    }
}