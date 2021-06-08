using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using UnityEngine;

namespace Basra.Client.Test
{
    class LogicLab
    {
        [Test]
        public void Eat()
        {
            Debug.Log(string.Join(" ,", Client.LogicLab.Eat(7, new int[] { 0, 0, 3, 3, 6, 7, 2 })));
            Debug.Log(string.Join(" ,", Client.LogicLab.Eat(7, new int[] { 14, 29, 0, 6, 16, 21 })));
            Debug.Log(string.Join(" ,", Client.LogicLab.Eat(9, new int[] { 14, 29, 0, 6, 16, 21 })));
        }
        [Test]
        public void GetCombination()
        {
            var list = new List<int> { 0, 1, 2, 3 };
            double count = Math.Pow(2, list.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                string str = Convert.ToString(i, 2).PadLeft(list.Count, '0');
                var debug = "";
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        debug += list[j] + ", ";
                    }
                }
                Debug.Log(debug);
            }
        }

        [Test]
        public void TestPer()
        {
            var res = Permutations(new List<int> { 0, 1, 2, 3 });
            foreach (var item in res)
            {
                Debug.Log(string.Join(", ", item));
            }
        }

        public static IEnumerable<T[]> Permutations<T>(IEnumerable<T> source)
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
    }
}
