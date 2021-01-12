using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Basra.Server.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

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

        [Fact]
        public async Task Test2()
        {
            var timer = new Timer(aa, "any type of object", 3000, Timeout.Infinite);
            await timer.DisposeAsync();
            if (timer == null)
                Debug.WriteLine("hellllllo");
            // timer.Elapsed += (sender, e) => TurnTimeout(sender, e, UserInTurn);
        }

        private void aa(object state)
        {
            _testOutputHelper.WriteLine((string)state);
            _testOutputHelper.WriteLine("dsafjhasjkdfkdfhasjk");
        }



    }
}