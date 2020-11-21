using System;
using System.Diagnostics;
using Xunit;

namespace Basra.Server.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var playerCount = 3;
            var deckCount = 13;

            var handSize = deckCount / playerCount;
            var remainder = deckCount % playerCount;

            Debug.WriteLine(handSize);
            Debug.WriteLine(remainder);
        }
    }
}
