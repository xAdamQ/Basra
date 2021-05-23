using System;
using Zenject;
using System.Collections;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using Moq;

namespace LobbyTests
{
    public class LobbyViewsTest : SceneTestFixture
    {
        [SetUp]
        private void SetupTestRepoUserData()
        {
            var personalInfo = new PersonalFullUserInfo
            {
                BasrasCount = 3,
                BigBasrasCount = 3,
                Level = 23,
                Money = 433,
                PlayedRoomsCount = 56,
                WonRoomsCount = 22,
                EatenCardsCount = 298,
                WinStreak = 3,
                Id = "tstId",
                DisplayName = "7oda el gamed",
                Title = "uooo7, I am so funky",
                Picture = Texture2D.redTexture,
                MoneyAimTimeLeft = TimeSpan.FromMinutes(2),
            };
            var topFriends = new[]
            {
                new MinUserInfo
                {
                    Id = "tstFriendId",
                    Level = 13,
                    DisplayName = "Hany Shaker",
                    Picture = Texture2D.grayTexture,
                    Title = "I am a normal human being"
                },
                new MinUserInfo
                {
                    Id = "tstFriendId2",
                    Level = 93,
                    DisplayName = "Emad Ta3ban",
                    Picture = Texture2D.linearGrayTexture,
                    Title = "abo fa45a"
                }
            };
            var yesterdayChampions = new[]
            {
                new MinUserInfo
                {
                    Id = "tstChampionId",
                    Level = 33,
                    DisplayName = "Ben Awad",
                    Picture = Texture2D.blackTexture,
                    Title = "SuperMan with good company"
                },
                new MinUserInfo
                {
                    Id = "tstChampionId2",
                    Level = 7,
                    DisplayName = "Ola Sameh",
                    Picture = Texture2D.normalTexture,
                    Title = "Take it easy"
                }
            };

            var controller = new Mock<IController>();
            controller.Setup(_ => _.GetPublicFullUserInfo(topFriends[0].Id)).Returns(UniTask.FromResult(
                new FullUserInfo
                {
                    BasrasCount = 3,
                    BigBasrasCount = 3,
                    Level = 23,
                    PlayedRoomsCount = 56,
                    WonRoomsCount = 22,
                    EatenCardsCount = 298,
                    WinStreak = 3,
                    Id = "tstId",
                    DisplayName = "7oda el gamed dsf",
                    Title = "uooo7, I am so funky sssssss",
                    Picture = Texture2D.redTexture,
                }));
            controller.Setup(_ => _.GetPublicFullUserInfo(topFriends[1].Id)).Returns(UniTask.FromResult(
                new FullUserInfo
                {
                    BasrasCount = 7,
                    BigBasrasCount = 1,
                    Level = 977,
                    PlayedRoomsCount = 561,
                    WonRoomsCount = 242,
                    EatenCardsCount = 2898,
                    WinStreak = 36,
                    Id = "tstId55",
                    DisplayName = "ss7soda el gamed",
                    Title = "uooo7, I adm so funky",
                    Picture = Texture2D.redTexture,
                }));

            StaticContext.Container.BindInstance(controller.Object);
            StaticContext.Container.BindInterfacesTo<Repository>().AsSingle();
            var repo = StaticContext.Container.Resolve<IRepository>();
            repo.PersonalFullInfo = personalInfo;
            repo.TopFriends = topFriends;
            repo.YesterdayChampions = yesterdayChampions;
        }

        [UnityTest]
        [Timeout(999999999)]
        public IEnumerator TestScene()
        {
            Debug.Log("ProjectContext is gathered" + ProjectContext.Instance.Container);

            yield return LoadScene("Lobby");

            yield return new WaitForSeconds(999999);

            // TODO: Add assertions here now that the scene has started
            // Or you can just uncomment to simply wait some time to make sure the scene plays without errors
            //yield return new WaitForSeconds(1.0f);

            // Note that you can use SceneContainer.Resolve to look up objects that you need for assertions
        }
    }
}