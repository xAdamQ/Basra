using System;
using System.Collections.Concurrent;
using Xunit;

namespace GeneralTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var PendingRooms = new ConcurrentDictionary<(int, int), string>();
            PendingRooms.TryAdd((13, 15), "13,15");
            PendingRooms.TryAdd((13, 17), "13,15");

            // PendingRooms.TryGetValue((13, 15), out string r1);
            // Console.WriteLine(r1);

            // PendingRooms.TryGetValue((13, ), out string r2);
        }
    }
}