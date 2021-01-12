using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Basra.Server.Room.Tests
{
    public class Logic
    {
        public static IEnumerable<object[]> BasraEat_ShouldWork_Data => new[]
        {
            new object[] { 48, new List<int> { 40, 42, 44, 47 } , new List<int> { 42, 44 }, false, false }, //normal
            new object[] { 18, new List<int> { 40, 42, 44, 47 } , new List<int> { 40, 42 }, false, false }, //normal
            new object[] { 36, new List<int> { 40, 42, 44, 47 } , new List<int> { 40, 42, 44, 47 }, false, false}, //boy
            new object[] { 19, new List<int> { 47 } , new List<int> { 47 }, true, false }, //basra komi
        };
        [Theory]
        [MemberData(nameof(BasraEat_ShouldWork_Data))]
        public void BasraEat_ShouldWork(int cardId, List<int> ground, List<int> eaten, bool basra, bool bigBasra)
        {
            var actualEaten = Room.Logic.Eat(cardId, ground, out bool actualBasra, out bool actualBigBasra);

            Assert.Equal(eaten, actualEaten);
            Assert.Equal(basra, actualBasra);
            Assert.Equal(bigBasra, actualBigBasra);
        }

    }
}
