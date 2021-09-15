// using System;
// using Zenject;
// using System.Collections;
// using System.ComponentModel;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.TestTools;
// using NUnit.Framework;
// using Moq;
//
// namespace LobbyTests
// {
//     public class LobbyViewsTest : SceneTestFixture
//     {
//         [SetUp]
//         private void SetupTestRepoUserData()
//         {
//             var personalInfo = new PersonalFullUserInfo
//             {
//                 BasraCount = 3,
//                 BigBasraCount = 3,
//                 Level = 23,
//                 Money = 433,
//                 PlayedRoomsCount = 56,
//                 WonRoomsCount = 22,
//                 EatenCardsCount = 298,
//                 WinStreak = 3,
//                 Id = "tstId",
//                 Name = "7oda el gamed",
//                 SelectedTitleId = 1,
//                 Picture = Texture2D.redTexture,
//                 MoneyAimTimeLeft = TimeSpan.FromMinutes(2),
//             };
//             var topFriends = new[]
//             {
//                 new MinUserInfo
//                 {
//                     Id = "tstFriendId",
//                     Level = 13,
//                     Name = "Hany Shaker",
//                     Picture = Texture2D.grayTexture,
//                     SelectedTitleId = 1,
//                 },
//                 new MinUserInfo
//                 {
//                     Id = "tstFriendId2",
//                     Level = 93,
//                     Name = "Emad Ta3ban",
//                     Picture = Texture2D.linearGrayTexture,
//                     SelectedTitleId = 1,
//                 }
//             };
//             var yesterdayChampions = new[]
//             {
//                 new MinUserInfo
//                 {
//                     Id = "tstChampionId",
//                     Level = 33,
//                     Name = "Ben Awad",
//                     Picture = Texture2D.blackTexture,
//                     SelectedTitleId = 1,
//                 },
//                 new MinUserInfo
//                 {
//                     Id = "tstChampionId2",
//                     Level = 7,
//                     Name = "Ola Sameh",
//                     Picture = Texture2D.normalTexture,
//                     SelectedTitleId = 1,
//                 }
//             };
//
//             var controller = new Mock<IController>();
//             controller.Setup(_ => _.GetPublicFullUserInfo(topFriends[0].Id)).Returns(UniTask.FromResult(
//                 new FullUserInfo
//                 {
//                     BasraCount = 3,
//                     BigBasraCount = 3,
//                     Level = 23,
//                     PlayedRoomsCount = 56,
//                     WonRoomsCount = 22,
//                     EatenCardsCount = 298,
//                     WinStreak = 3,
//                     Id = "tstId",
//                     Name = "7oda el gamed dsf",
//                     SelectedTitleId = 1,
//                     Picture = Texture2D.redTexture,
//                 }));
//             controller.Setup(_ => _.GetPublicFullUserInfo(topFriends[1].Id)).Returns(UniTask.FromResult(
//                 new FullUserInfo
//                 {
//                     BasraCount = 7,
//                     BigBasraCount = 1,
//                     Level = 977,
//                     PlayedRoomsCount = 561,
//                     WonRoomsCount = 242,
//                     EatenCardsCount = 2898,
//                     WinStreak = 36,
//                     Id = "tstId55",
//                     Name = "ss7soda el gamed",
//                     SelectedTitleId = 1,
//                     Picture = Texture2D.redTexture,
//                 }));
//
//             StaticContext.Container.BindInstance(controller.Object);
//             StaticContext.Container.BindInterfacesTo<Repository>().AsSingle();
//             var repo = StaticContext.Container.Resolve<IRepository>();
//             repo.PersonalFullInfo = personalInfo;
//             repo.TopFriends = topFriends;
//             repo.YesterdayChampions = yesterdayChampions;
//         }
//
//         [UnityTest]
//         [Timeout(999999999)]
//         public IEnumerator TestScene()
//         {
//             Debug.Log("ProjectContext is gathered" + ProjectContext.Instance.Container);
//
//             yield return LoadScene("Lobby");
//
//             yield return new WaitForSeconds(999999);
//
//             // TODO: Add assertions here now that the scene has started
//             // Or you can just uncomment to simply wait some time to make sure the scene plays without errors
//             //yield return new WaitForSeconds(1.0f);
//
//             // Note that you can use SceneContainer.Resolve to look up objects that you need for assertions
//         }
//         
//         
//     }
// }

