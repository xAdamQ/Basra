// using System;
// using Zenject;
// using System.Collections;
// using Cysharp.Threading.Tasks;
// using Moq;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// public class RoomTests : SceneTestFixture
// {
//     [UnityTest]
//     [Timeout(999999999)]
//     public IEnumerator GameplayTest()
//     {
//         var personalInfo = new PersonalFullUserInfo
//         {
//             BasrasCount = 3,
//             BigBasrasCount = 3,
//             Level = 23,
//             Money = 433,
//             PlayedRoomsCount = 56,
//             WonRoomsCount = 22,
//             EatenCardsCount = 298,
//             WinStreak = 3,
//             Id = "tstId",
//             DisplayName = "7oda el gamed",
//             Title = "uooo7, I am so funky",
//             Picture = Texture2D.redTexture,
//             MoneyAimTimeLeft = TimeSpan.FromMinutes(2),
//         };
//         var topFriends = new[]
//         {
//             new MinUserInfo
//             {
//                 Id = "tstFriendId",
//                 Level = 13,
//                 DisplayName = "Hany Shaker",
//                 Picture = Texture2D.grayTexture,
//                 Title = "I am a normal human being"
//             },
//             new MinUserInfo
//             {
//                 Id = "tstFriendId2",
//                 Level = 93,
//                 DisplayName = "Emad Ta3ban",
//                 Picture = Texture2D.linearGrayTexture,
//                 Title = "abo fa45a"
//             }
//         };
//         var yesterdayChampions = new[]
//         {
//             new MinUserInfo
//             {
//                 Id = "tstChampionId",
//                 Level = 33,
//                 DisplayName = "Ben Awad",
//                 Picture = Texture2D.blackTexture,
//                 Title = "SuperMan with good company"
//             },
//             new MinUserInfo
//             {
//                 Id = "tstChampionId2",
//                 Level = 7,
//                 DisplayName = "Ola Sameh",
//                 Picture = Texture2D.normalTexture,
//                 Title = "Take it easy"
//             }
//         };
//
//         var controller = new Mock<IController>();
//         controller.Setup(_ => _.GetPublicFullUserInfo(topFriends[0].Id)).Returns(UniTask.FromResult(
//             new FullUserInfo
//             {
//                 BasrasCount = 3,
//                 BigBasrasCount = 3,
//                 Level = 23,
//                 PlayedRoomsCount = 56,
//                 WonRoomsCount = 22,
//                 EatenCardsCount = 298,
//                 WinStreak = 3,
//                 Id = "tstId",
//                 DisplayName = "7oda el gamed dsf",
//                 Title = "uooo7, I am so funky sssssss",
//                 Picture = Texture2D.redTexture,
//             }));
//         controller.Setup(_ => _.GetPublicFullUserInfo(topFriends[1].Id)).Returns(UniTask.FromResult(
//             new FullUserInfo
//             {
//                 BasrasCount = 7,
//                 BigBasrasCount = 1,
//                 Level = 977,
//                 PlayedRoomsCount = 561,
//                 WonRoomsCount = 242,
//                 EatenCardsCount = 2898,
//                 WinStreak = 36,
//                 Id = "tstId55",
//                 DisplayName = "ss7soda el gamed",
//                 Title = "uooo7, I adm so funky",
//                 Picture = Texture2D.redTexture,
//             }));
//
//         controller.Setup(_ => _.SendAsync(It.IsAny<string>())).Returns<string, object[]>((method, args) =>
//         {
//             Debug.Log($"mocking call {method} in the server with args {string.Join(", ", args)}");
//             return UniTask.FromResult(new object());
//         });
//
//         StaticContext.Container.BindInstance(controller.Object);
//         StaticContext.Container.BindInterfacesTo<Repository>().AsSingle();
//         var repo = StaticContext.Container.Resolve<IRepository>();
//         repo.PersonalFullInfo = personalInfo;
//         repo.TopFriends = topFriends;
//         repo.YesterdayChampions = yesterdayChampions;
//
//         var opposInfo = new[]
//         {
//             new FullUserInfo
//             {
//                 Id = "tstFriendId",
//                 Level = 13,
//                 DisplayName = "Hany Shaker",
//                 Picture = Texture2D.grayTexture,
//                 Title = "I am a normal human being"
//             },
//             new FullUserInfo
//             {
//                 Id = "tstFriendId2",
//                 Level = 93,
//                 DisplayName = "Emad Ta3ban",
//                 Picture = Texture2D.linearGrayTexture,
//                 Title = "abo fa45a"
//             }
//         };
//
//         StaticContext.Container.BindInstance(new RoomController.Settings
//         {
//             BetChoice = 0,
//             CapacityChoice = 0,
//             OpposInfo = opposInfo,
//         }).WhenInjectedInto<IRoomController>();
//
//         StaticContext.Container.BindInstance(false);
//
//
//         yield return LoadScene("Room");
//
//         yield return new WaitForSeconds(999999);
//     }
// }

