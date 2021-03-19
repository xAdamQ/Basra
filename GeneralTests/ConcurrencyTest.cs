using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace GeneralTests
{
    public class ConcurrencyTest
    {
        public ConcurrencyTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void IntInNoInterlock()
        {
            var thread1 = new Thread(Inc10000);
            var thread2 = new Thread(Inc10000);
            var thread3 = new Thread(Inc10000);

            thread1.Start();
            thread2.Start();
            thread3.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
        }

        private int x = 0;
        private ITestOutputHelper _testOutputHelper;
        private object _lock = new object();
        private HashSet<int> saved = new HashSet<int>();

        private void Inc10000()
        {
            for (int i = 0; i < 100; i++)
            {
                // lock (_lock)
                // {
                //     x++;
                //     _testOutputHelper.WriteLine(x.ToString());
                // }

                try
                {
                    var a = Interlocked.Increment(ref x);
                    if (saved.Contains(a)) _testOutputHelper.WriteLine($"{a} exists");
                    else saved.Add(a);

                    _testOutputHelper.WriteLine(a.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                // _testOutputHelper.WriteLine("inter: " + Interlocked.Increment(ref x).ToString());
                // _testOutputHelper.WriteLine("non; " + x.ToString());
            }
        }

        private void ConcBag()
        {
            var dic = new ConcurrentDictionary<int, string>();
            // var bag = new ConcurrentBag<int>();
            dic.TryGetValue(12, out string vall);
        }
    }
}