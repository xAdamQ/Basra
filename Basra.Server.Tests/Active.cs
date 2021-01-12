using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace Basra.Server.Room.Tests
{
    public class Active
    {
        public static IEnumerable<object[]> BasraEat_ShouldWork_Data => new[]
        {
            new object[] {
                3, 1, new int[] { 13, 22, 2 }, new int[] { 0, 0, 0}, new int[] { 0, 0, 0},
                new int[]{0, 300, 0}, new int[]{0, 0, 0}, new int[]{0, 1, 0}
            },
             new object[] {
                3, 1, new int[] { 14, 14, 2 }, new int[] { 0, 0, 0}, new int[] { 0, 0, 0},
                new int[]{150, 150, 0}, new int[]{1, 1, 0}, new int[]{0, 0, 0}
            },
            new object[] {
                3, 0, new int[] { 14, 14, 2 }, new int[] { 0, 0, 0}, new int[] { 0, 0, 0},
                new int[]{75, 75, 0}, new int[]{1, 1, 0}, new int[]{0, 0, 0}
            },
            new object[] {
                3, 0, new int[] { 14, 14, 2 }, new int[] { 0, 0, 0}, new int[] { 0, 0, 0},
                new int[]{75, 75, 0}, new int[]{1, 1, 0}, new int[]{0, 0, 0}
            },

        };
        [Theory]
        [MemberData(nameof(BasraEat_ShouldWork_Data))]
        public void FinalizeGame_ShouldWork(
            int userCount, int genre, int[] eatenCardsLengths, int[] basrasCount, int[] bigBasrasCount,
            int[] addedMoney, int[] addedDraws, int[] addedWins)
        {
            //arrange
            var pending = new Mock<IPending>();
            pending.Setup(p => p.Genre).Returns(genre);
            pending.Setup(p => p.PlayerCount).Returns(userCount);

            var users = new Mock<IUser>[userCount];
            for (int u = 0; u < userCount; u++)
            {
                users[u] = new Mock<IUser>();
                users[u].SetupAllProperties();

                users[u].Setup(u => u.EatenCardsCount).Returns(eatenCardsLengths[u]);
                users[u].Setup(u => u.BasraCount).Returns(basrasCount[u]);
                users[u].Setup(u => u.BigBasraCount).Returns(bigBasrasCount[u]);

                users[u].Setup(u => u.Structure.PlayedGames).Returns(0);
                users[u].Setup(u => u.Structure.Wins).Returns(0);
                users[u].Setup(u => u.Structure.Draws).Returns(0);
            }

            pending.Setup(p => p.Users).Returns(users.Select(u => u.Object).ToList());

            var active = new Room.Active(pending.Object);

            //act
            active.FinalizeGame();

            //assert
            var addedPlayedGames = 1;
            for (int u = 0; u < userCount; u++)
            {
                Assert.Equal(active.Users[u].Structure.PlayedGames, addedPlayedGames);
                Assert.Equal(active.Users[u].Structure.Money, addedMoney[u]);
                Assert.Equal(active.Users[u].Structure.Draws, addedDraws[u]);
                Assert.Equal(active.Users[u].Structure.Wins, addedWins[u]);
            }
        }

    }
}
