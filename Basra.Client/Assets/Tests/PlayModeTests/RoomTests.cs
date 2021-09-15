// using Cysharp.Threading.Tasks;
// // using Basra.Models.Client;
// using DG.Tweening;
// using Moq;
// using NUnit.Framework;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.TestTools;
// using Zenject;
// using Object = UnityEngine.Object;
//
// namespace PlayModeTests
// {
//     [TestFixture]
//     public class RoomTests : RoomTestBase
//     {
//         [SetUp]
//         public void Install()
//         {
//             ProjectContext.Instance.Container.Inject(this);
//             Container.BindInstance(new RoomSettings(0, 0, fullUserInfos.ToList(), 0)).AsSingle();
//             // .WhenInjectedInto<RoomInstaller>();
//         }
//
//         [Inject] private ZenjectSceneLoader _zenjectSceneLoader;
//
//         [UnityTest]
//         public IEnumerator overallGame()
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
//                 SelectedTitleId = 0,
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
//                     SelectedTitleId = 2,
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
//                     SelectedTitleId = 3,
//                 },
//                 new MinUserInfo
//                 {
//                     Id = "tstChampionId2",
//                     Level = 7,
//                     Name = "Ola Sameh",
//                     Picture = Texture2D.normalTexture,
//                     SelectedTitleId = 0,
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
//                     SelectedTitleId = 2,
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
//             controller.Setup(_ => _.SendAsync(It.IsAny<string>())).Returns<string, object[]>((method, args) =>
//             {
//                 Debug.Log($"mocking call {method} in the server with args {string.Join(", ", args)}");
//                 return UniTask.FromResult(new object());
//             });
//
//             StaticContext.Container.BindInstance(controller.Object);
//             StaticContext.Container.BindInterfacesTo<Repository>().AsSingle();
//             var repo = StaticContext.Container.Resolve<IRepository>();
//             repo.PersonalFullInfo = personalInfo;
//             repo.TopFriends = topFriends;
//             repo.YesterdayChampions = yesterdayChampions;
//
//             var opposInfo = new List<FullUserInfo>()
//             {
//                 new FullUserInfo
//                 {
//                     Id = "tstFriendId",
//                     Level = 13,
//                     Name = "Hany Shaker",
//                     Picture = Texture2D.grayTexture,
//                     SelectedTitleId = 1,
//                 },
//                 new FullUserInfo
//                 {
//                     Id = "tstFriendId2",
//                     Level = 93,
//                     Name = "Emad Ta3ban",
//                     Picture = Texture2D.linearGrayTexture,
//                     SelectedTitleId = 1,
//                 },
//             };
//
//
//             StaticContext.Container.BindInstance(new RoomSettings(0, 0, opposInfo, 0))
//                 .WhenInjectedInto<RoomSettings>();
//
//             StaticContext.Container.BindInstance(false);
//
//             _zenjectSceneLoader.LoadScene("Room", extraBindings: container =>
//             {
//                 container.BindInstance(new RoomInstaller.ModuleSwitches(true)
//                 {
//                     EnablePlayerBaseFactory = true,
//                     EnableRoomUserViewFactory = true,
//                 });
//             });
//
//             yield return new WaitForSeconds(999999);
//         }
//
//         private DiContainer GetSceneDiContainer()
//         {
//             return ProjectContext.Instance.Container.Resolve<SceneContextRegistry>()
//                 .TryGetSceneContextForScene("Room").Container;
//         }
//
//         [UnityTest]
//         public IEnumerator blank()
//         {
//             var frontSprites = Addressables.LoadAssetAsync<Sprite[]>("backSprites");
//
//             yield return new WaitUntil(() => frontSprites.IsDone);
//
//             Debug.Log(frontSprites.Result.Length);
//             Debug.Log(frontSprites.Result[0]);
//
//             yield return new WaitForSeconds(float.MaxValue);
//         }
//
//         private void InstallMockRoomController()
//         {
//             var roomController = new Mock<IRoomController>();
//             Container.Bind<IRoomController>().FromInstance(roomController.Object);
//         }
//
//         static PersonalFullUserInfo personalInfo = new PersonalFullUserInfo
//         {
//             BasraCount = 3,
//             BigBasraCount = 3,
//             Level = 23,
//             Money = 433,
//             PlayedRoomsCount = 56,
//             WonRoomsCount = 22,
//             EatenCardsCount = 298,
//             WinStreak = 3,
//             Id = "tstId",
//             Name = "7oda el gamed",
//             SelectedTitleId = 1,
//             Picture = Texture2D.redTexture,
//             MoneyAimTimeLeft = TimeSpan.FromMinutes(2),
//         };
//
//         static MinUserInfo[] topFriends = new[]
//         {
//             new MinUserInfo
//             {
//                 Id = "tstFriendId",
//                 Level = 13,
//                 Name = "Hany Shaker",
//                 Picture = Texture2D.grayTexture,
//                 SelectedTitleId = 1,
//             },
//             new MinUserInfo
//             {
//                 Id = "tstFriendId2",
//                 Level = 93,
//                 Name = "Emad Ta3ban",
//                 Picture = Texture2D.linearGrayTexture,
//                 SelectedTitleId = 1,
//             },
//             new MinUserInfo
//             {
//                 Id = "tstFriendId2",
//                 Level = 93,
//                 Name = "Emad Ta3ban",
//                 Picture = Texture2D.linearGrayTexture,
//                 SelectedTitleId = 1,
//             }
//         };
//
//         static MinUserInfo[] yesterdayChampions = new[]
//         {
//             new MinUserInfo
//             {
//                 Id = "tstChampionId",
//                 Level = 33,
//                 Name = "Ben Awad",
//                 Picture = Texture2D.blackTexture,
//                 SelectedTitleId = 1,
//             },
//             new MinUserInfo
//             {
//                 Id = "tstChampionId2",
//                 Level = 7,
//                 Name = "Ola Sameh",
//                 Picture = Texture2D.normalTexture,
//                 SelectedTitleId = 1,
//             }
//         };
//
//         static FullUserInfo[] fullUserInfos = new[]
//         {
//             new FullUserInfo
//             {
//                 BasraCount = 3,
//                 BigBasraCount = 3,
//                 Level = 23,
//                 PlayedRoomsCount = 56,
//                 WonRoomsCount = 22,
//                 EatenCardsCount = 298,
//                 WinStreak = 3,
//                 Id = "tstId",
//                 Name = "7oda el gamed",
//                 SelectedTitleId = 1,
//                 Picture = Texture2D.redTexture,
//                 SelectedCardback = 7,
//             },
//             new FullUserInfo
//             {
//                 BasraCount = 3,
//                 BigBasraCount = 3,
//                 Level = 23,
//                 PlayedRoomsCount = 56,
//                 WonRoomsCount = 22,
//                 EatenCardsCount = 298,
//                 WinStreak = 3,
//                 Id = "tstId",
//                 Name = "7oda el gamed",
//                 SelectedTitleId = 1,
//                 Picture = Texture2D.redTexture,
//                 SelectedCardback = 2,
//             },
//             new FullUserInfo
//             {
//                 BasraCount = 3,
//                 BigBasraCount = 3,
//                 Level = 23,
//                 PlayedRoomsCount = 56,
//                 WonRoomsCount = 22,
//                 EatenCardsCount = 298,
//                 WinStreak = 3,
//                 Id = "tstId",
//                 Name = "7oda el gamed",
//                 SelectedTitleId = 1,
//                 Picture = Texture2D.redTexture,
//                 SelectedCardback = 5,
//             },
//             new FullUserInfo
//             {
//                 BasraCount = 3,
//                 BigBasraCount = 3,
//                 Level = 23,
//                 PlayedRoomsCount = 56,
//                 WonRoomsCount = 22,
//                 EatenCardsCount = 298,
//                 WinStreak = 3,
//                 Id = "tstId",
//                 Name = "7oda el gamed",
//                 SelectedTitleId = 1,
//                 Picture = Texture2D.redTexture,
//                 SelectedCardback = 9,
//             },
//         };
//
//
//         private void InstallMockController()
//         {
//             var controller = new Mock<IController>();
//
//
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
//             controller.Setup(_ => _.SendAsync(It.IsAny<string>())).Returns<string, object[]>((method, args) =>
//             {
//                 Debug.Log($"mocking call {method} in the server with args {string.Join(", ", args)}");
//                 return UniTask.FromResult(new object());
//             });
//
//             controller.Setup(_ => _.ThrowCard(It.IsAny<int>()));
//             // .Returns(UniTask.FromResult(new ThrowResult { EatenCardsIds = new List<int>(), Basra = true }));
//             //* the logic here has changed
//
//             controller.Setup(_ => _.NotifyTurnMiss()).Callback(() => Debug.Log("notify turn miss called"));
//
//             Container.BindInstance(controller.Object);
//         }
//         private void InstallMockGround()
//         {
//             var ground = new Mock<IGround>();
//             ground.Setup(_ =>
//                     _.Throw(It.IsAny<Card>(), It.IsAny<List<int>>(), It.IsAny<Sequence>(), It.IsAny<Vector2?>()))
//                 .Callback<Card, int[]>((c, arr) => Object.Destroy(c));
//             Container.Bind<IGround>().FromMock();
//         }
//         private void InstallRepository()
//         {
//             StaticContext.Container.BindInterfacesTo<Repository>().AsSingle();
//             var repo = StaticContext.Container.Resolve<IRepository>();
//             repo.PersonalFullInfo = personalInfo;
//             repo.TopFriends = topFriends;
//             repo.YesterdayChampions = yesterdayChampions;
//         }
//
//         [UnityTest]
//         public IEnumerator Oppo_Throw()
//         {
//             yield break;
//             //
//             // InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             // {
//             //     EnableCardFactory = true,
//             //     EnableFrontFactory = true,
//             //     EnablePlayerBaseFactory = true,
//             //     EnableGround = true,
//             // });
//             //
//             // InstallMockRoomController();
//             // InstallMockController();
//             //
//             // var fac = Container.Resolve<PlayerBase.Factory>();
//             // var oppo = (IOppo) fac.Create(1, 0);
//             //
//             // oppo.Distribute();
//             //
//             // // oppo.Throw(new ThrowResult { ThrownCard = 0, EatenCardsIds = new List<int>(), BigBasra = true });
//             // // yield return new WaitForSeconds(3);
//             // // oppo.Throw(1, new ThrowResult { ThrownCard = 1, EatenCardsIds = new List<int>(), Basra = true });
//             // // yield return new WaitForSeconds(3);
//             // // oppo.Throw(2, new ThrowResult { EatenCardsIds = new List<int>() });
//             // // yield return new WaitForSeconds(3);
//             // // oppo.Throw(2, new ThrowResult { EatenCardsIds = new List<int>() });
//             //
//             // // Assert.Throws(typeof(Exception), () => oppo.Throw(2, new ThrowResponse {EatenCardsIds = new List<int>()}));
//             //
//             // yield return new WaitForSeconds(999999);
//         }
//
//         [UnityTest]
//         public IEnumerator Ground_Throw()
//         {
//             InstallProjectModule(new ProjectInstaller.Settings(false) { });
//             InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             {
//                 EnableGround = true,
//                 EnableCardFactory = true,
//                 EnableFrontFactory = true,
//             });
//
//             var ground = Container.Resolve<IGround>();
//             ground.Distribute(new List<int>() {1, 2, 3, 4});
//
//             var cFac = Container.Resolve<Card.Factory>();
//
//             yield return new WaitForSeconds(2);
//             ground.Throw(cFac.CreateGroundCard(11, null), new List<int>(), DOTween.Sequence(), Vector2.zero);
//             yield return new WaitForSeconds(2);
//             ground.Throw(cFac.CreateGroundCard(11, null), new List<int>(), DOTween.Sequence(), Vector2.zero);
//             yield return new WaitForSeconds(2);
//         }
//
//         [UnityTest]
//         public IEnumerator Ground_Throw_ShouldEatRight()
//         {
//             InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             {
//                 EnableGround = true,
//                 EnableCardFactory = true,
//                 EnableFrontFactory = true,
//             });
//
//             var ground = Container.Resolve<IGround>();
//             ground.Distribute(new List<int>() {1, 2, 3, 4});
//
//             var cFac = Container.Resolve<Card.Factory>();
//
//
//             yield return new WaitForSeconds(2);
//             ground.Throw(cFac.CreateGroundCard(11, null), new List<int> {1}, null, null);
//             yield return new WaitForSeconds(2);
//             ground.Throw(cFac.CreateGroundCard(11, null), new List<int> {2, 3}, null, null);
//             yield return new WaitForSeconds(2);
//             ground.Throw(cFac.CreateGroundCard(11, null), new List<int>(), null, null);
//             yield return new WaitForSeconds(2);
//             ground.Throw(cFac.CreateGroundCard(11, null), new List<int>(), null, null);
//             yield return new WaitForSeconds(2);
//             ground.Throw(cFac.CreateGroundCard(11, null), new List<int> {11, 11}, null, null);
//
//
//             Assert.Throws(typeof(Exception),
//                 () => { ground.Throw(cFac.CreateGroundCard(11, null), new List<int> {2}, null, null); });
//             yield return new WaitForSeconds(2);
//         }
//
//         [UnityTest]
//         //broken
//         public IEnumerator TurnTimer_ShouldUpdateAndFireOnComplete()
//         {
//             InstallProjectModule(new ProjectInstaller.Settings(false)
//             {
//             });
//
//             // Container.BindInstance(new RoomSettings(0, 0, null, 0)).AsSingle().WhenInjectedInto<RoomInstaller>();
//
//             InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             {
//                 // EnableTurnTimer = true,
//             });
//
//             //room controller factory requires the "facade" room controller
//
//             var tt = Container.Resolve<ITurnTimer>();
//
//             yield return null;
//
//             tt.Elapsed += () => Debug.Log("timer is done");
//
//             tt.Play();
//
//             Assert.True(tt.IsPlaying);
//
//             yield return new WaitUntil(() => !tt.IsPlaying);
//         }
//
//         [UnityTest]
//         //broken
//         public IEnumerator TurnTimer_ShouldOverrideTimers()
//         {
//             // Container.BindInstance(new RoomSettings(0, 0, null, 0)).AsSingle().WhenInjectedInto<RoomInstaller>();
//
//             InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             {
//                 // EnableTurnTimer = true,
//             });
//
//             var tt = Container.Resolve<ITurnTimer>();
//             tt.Elapsed += () => Debug.Log("timer is done");
//
//             yield return null;
//
//             tt.Play();
//             yield return new WaitForSeconds(2);
//             tt.Play();
//             Assert.True(tt.IsPlaying);
//
//             tt.Stop();
//             Assert.False(tt.IsPlaying);
//
//             tt.Play();
//             Assert.True(tt.IsPlaying);
//             yield return new WaitForSeconds(10);
//             Assert.False(tt.IsPlaying);
//         }
//
//         [UnityTest]
//         public IEnumerator RoomUserViews_ShouldAllShowInPlaceWithRightData()
//         {
//             Container.Bind<IRepository>().FromMock();
//             Container.Bind<IController>().FromMock();
//
//             InstallProjectModule(new ProjectInstaller.Settings(false)
//             {
//                 EnableToast = true,
//                 EnableBlockingPanel = true,
//                 EnableBlockingOperationManager = true,
//                 EnableFullUserView = true,
//             });
//
//             InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             {
//                 EnableRoomUserViewFactory = true,
//             });
//
//             var viewFac = Container.Resolve<RoomUserView.IManager>();
//
//             var infos = new List<FullUserInfo> {personalInfo};
//             infos.AddRange(fullUserInfos);
//             viewFac.Init(infos, 0);
//
//             yield return new WaitForSeconds(float.MaxValue);
//         }
//
//         [UnityTest]
//         public IEnumerator RoomUserViews_ShouldAllShowFullInfo()
//         {
//             Container.Bind<IRepository>().FromMock();
//             Container.Bind<IController>().FromMock();
//
//             InstallProjectModule(new ProjectInstaller.Settings(false)
//             {
//                 EnableToast = true,
//                 EnableBlockingPanel = true,
//                 EnableBlockingOperationManager = true,
//                 EnableFullUserView = true,
//             });
//
//             InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             {
//                 EnableRoomUserViewFactory = true,
//             });
//
//             var viewFac = Container.Resolve<RoomUserView.IManager>();
//
//             var infos = new List<FullUserInfo> {personalInfo};
//             infos.AddRange(fullUserInfos);
//             viewFac.Init(infos, 0);
//
//             yield return new WaitForSeconds(float.MaxValue);
//         }
//
//
//         //[UnityTest]
//         //public IEnumerator RoomResultPanel_Interactive()
//         //{
//         //    InstallProjectModule(new ProjectInstaller.Settings(false)
//         //    {
//         //        EnableToast = true,
//         //        EnableBlockingPanel = true,
//         //        EnableRoomFactory = true,
//         //        EnableLobbyFactory = true,
//         //        EnableBlockingOperationManager = true,
//         //        EnableFullUserView = true,
//
//         //    });
//
//         //    InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//         //    {
//         //        EnableGround = true,
//         //        EnableCardFactory = true,
//         //        EnableFrontFactory = true,
//         //        EnableReferenceInstantiator = true,
//         //        EnableRoomController = true,
//         //        EnableRoomUserViewFactory = true,
//         //        EnablePlayerBaseFactory = true
//         //    });
//
//         //    Container.Bind<IController>().FromMock();
//         //    Container.Bind<ICoreGameplay>().FromMock();
//         //    Container.Bind<IRepository>().FromMock();
//
//         //    yield return null;
//
//         //    var refe = Container.Resolve<FinalizeInstaller.References>();
//         //    var ins = Container.Resolve<ReferenceInstantiator<RoomInstaller>>();
//
//         //    RoomResultPanel.Instantiate(ins, refe,
//         //        new RoomXpReport { Basra = 100, BigBasra = 4000, Competition = 9019, GreatEat = 127783 }, new PersonalFullUserInfo()
//         //        { PlayedRoomsCount = 10, WonRoomsCount = 9 },
//         //        personalInfo, 0);
//
//         //    yield return new WaitForSeconds(float.MaxValue);
//         //}
//
//
//         [UnityTest]
//         public IEnumerator AnimSequence()
//         {
//             yield break;
//             // InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             // {
//             //     EnableCardFactory = true,
//             //     EnableFrontFactory = true,
//             // });
//             //
//             // yield return null;
//             //
//             // var fac = Container.Resolve<Card.Factory>();
//             //
//             // var card = fac.CreateOppoCard(Sprite.Create(Texture2D.redTexture), null);
//             // var card2 = fac.CreateOppoCard(null);
//             // card2.transform.position = Vector3.right;
//             // var card3 = fac.CreateOppoCard(null);
//             // card3.transform.position = Vector3.left;
//             // var card4 = fac.CreateOppoCard(null);
//             // card4.transform.position = Vector3.up;
//             //
//             // var seq = DOTween.Sequence();
//             //
//             // seq
//             //     .Append(card.transform.DOScale(Vector3.zero, 1f).From())
//             //     .Append(card.transform.DOScale(Vector3.one * .5f, 1f))
//             //     ;
//             //
//             // var duration = seq.Duration();
//             //
//             //
//             // seq.Insert(duration, card2.transform.DOScale(Vector3.one * .5f, 1f));
//             // seq.Insert(duration, card3.transform.DOScale(Vector3.one * .5f, 1f));
//             // seq.Append(card4.transform.DOScale(Vector3.one * .5f, 1f));
//             //
//             //
//             // yield return new WaitForSeconds(6);
//         }
//
//         //---------------------------------------------------------
//
//         [UnityTest]
//         public IEnumerator InteractiveGpCore()
//         {
//             yield break;
//             //
//             // InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             // {
//             //     EnableCardFactory = true,
//             //     EnableFrontFactory = true,
//             //     EnablePlayerBaseFactory = true,
//             //     EnableGround = true,
//             // });
//             //
//             // Container.Bind<IRoomController>().FromMock();
//             // Container.Bind<IController>().FromMock();
//             //
//             // var tt = new Mock<ITurnTimer>();
//             // tt.Setup(_ => _.IsPlaying).Returns(true);
//             // Container.BindInstance(tt.Object);
//             //
//             // var core = new Mock<ICoreGameplay>();
//             // Container.BindInstance(core.Object);
//             //
//             // var fac = Container.Resolve<PlayerBase.Factory>();
//             // var g = Container.Resolve<IGround>();
//             //
//             // var player = (IPlayer) fac.Create(0, 0);
//             // core.Setup(_ => _.PlayerInTurn).Returns((PlayerBase) player);
//             //
//             // g.Distribute(new List<int>() {1, 1, 3, 3, 2, 6, 7});
//             // player.Distribute(new List<int>() {1, 2, 3, 4});
//             //
//             // yield return new WaitForSeconds(999999);
//         }
//
//
//         [UnityTest]
//         public IEnumerator GPCore_NextTurn()
//         {
//             InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             {
//                 EnableCardFactory = true,
//                 EnableFrontFactory = true,
//                 EnablePlayerBaseFactory = true,
//                 EnableCoreGameplay = true,
//             });
//
//             Container.Bind<IRoomController>().FromMock();
//             Container.Bind<IController>().FromMock();
//             Container.Bind<IRepository>().FromMock();
//             Container.Bind<IGround>().FromMock();
//
//             var fac = Container.Resolve<PlayerBase.Factory>();
//
//             var core = Container.Resolve<ICoreGameplay>();
//
//             core.CreatePlayers();
//             core.InitialTurn();
//
//             yield return new WaitForSeconds(2);
//
//             core.NextTurn();
//             yield return new WaitForSeconds(2);
//
//             core.NextTurn();
//             yield return new WaitForSeconds(10);
//
//             core.NextTurn();
//
//             yield return new WaitForSeconds(float.MaxValue);
//         }
//
//         [UnityTest]
//         public IEnumerator Player_Distribute() => UniTask.ToCoroutine(async () =>
//         {
//             InstallProjectModule(new ProjectInstaller.Settings(false)
//             {
//                 EnableToast = true,
//                 EnableBlockingPanel = true,
//                 EnableBlockingOperationManager = true,
//                 EnableFullUserView = true,
//             });
//
//             InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             {
//                 EnablePlayerBaseFactory = true,
//                 EnableCardFactory = true,
//                 EnableGround = true,
//                 EnableFrontFactory = true,
//                 EnableRoomUserViewFactory = true,
//             });
//
//             Container.Bind<ICoreGameplay>().FromMock();
//             Container.Bind<IController>().FromMock();
//             Container.Bind<IRepository>().FromMock();
//
//             await UniTask.DelayFrame(3);
//
//             var fac = Container.Resolve<PlayerBase.Factory>();
//             var fac2 = Container.Resolve<RoomUserView.IManager>();
//
//             fac2.Init(fullUserInfos.ToList(), 0);
//
//             var oppo1 = (IOppo) await fac.Create(4, 1, 1);
//             oppo1.Distribute();
//
//             var oppo2 = (IOppo) await fac.Create(8, 2, 2);
//             oppo2.Distribute();
//
//             var oppo3 = (IOppo) await fac.Create(10, 3, 3);
//             oppo3.Distribute();
//
//             await UniTask.Delay(2000);
//             oppo1.Throw(new ThrowResult { });
//             await UniTask.Delay(2000);
//             oppo1.Throw(new ThrowResult { });
//             await UniTask.Delay(2000);
//             oppo2.Throw(new ThrowResult { });
//             await UniTask.Delay(2000);
//             oppo3.Throw(new ThrowResult { });
//             await UniTask.Delay(2000);
//             oppo3.Throw(new ThrowResult { });
//             await UniTask.Delay(2000);
//             oppo3.Throw(new ThrowResult { });
//             await UniTask.Delay(2000);
//         });
//
//         [UnityTest]
//         public IEnumerator Player_Turn()
//         {
//             yield break;
//             // InstallProjectModule(new ProjectInstaller.Settings(false)
//             // {
//             //     EnableToast = true,
//             //     EnableBlockingPanel = true,
//             //     EnableBlockingOperationManager = true,
//             //     EnableFullUserView = true,
//             // });
//             // InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             // {
//             //     EnablePlayerBaseFactory = true,
//             //     EnableCardFactory = true,
//             //     EnableGround = true,
//             //     EnableFrontFactory = true,
//             //     EnableRoomUserViewFactory = true,
//             // });
//             // Container.Bind<ICoreGameplay>().FromMock();
//             // Container.Bind<IController>().FromMock();
//             // Container.Bind<IRepository>().FromMock();
//             // yield return null;
//             // var fac = Container.Resolve<PlayerBase.Factory>();
//             // var fac2 = Container.Resolve<RoomUserView.IManager>();
//             // var infos = new List<FullUserInfo> {personalInfo};
//             // infos.AddRange(fullUserInfos);
//             // fac2.Init(infos, 0);
//             // var player = (IPlayer) fac.Create(0, 0);
//             // var oppo1 = (IOppo) fac.Create(1, 1);
//             // var oppo2 = (IOppo) fac.Create(2, 2);
//             // var oppo3 = (IOppo) fac.Create(3, 3);
//             // yield return null;
//             // oppo1.Distribute();
//             // oppo2.Distribute();
//             // oppo3.Distribute();
//             // player.Distribute(new List<int>
//             // {
//             //     0, 1, 2, 3
//             // });
//             // yield return null;
//             // oppo1.StartTurn();
//             // yield return new WaitForSeconds(5);
//             // oppo1.EndTurn();
//             // player.StartTurn();
//             // yield return new WaitForSeconds(5);
//             // player.EndTurn();
//             // oppo2.StartTurn();
//             // yield return new WaitForSeconds(5);
//             // oppo2.EndTurn();
//             // oppo3.StartTurn();
//             // yield return new WaitForSeconds(5);
//             // oppo3.EndTurn();
//             // oppo1.StartTurn();
//             // yield return new WaitForSeconds(5);
//             // oppo1.EndTurn();
//         }
//
//         [UnityTest]
//         public IEnumerator LastEat()
//         {
//             yield break;
//             //
//             // InstallProjectModule(new ProjectInstaller.Settings(false)
//             // {
//             //     EnableBlockingOperationManager = true,
//             //     EnableBlockingPanel = true,
//             //     EnableToast = true,
//             //     EnableFullUserView = true,
//             // });
//             // InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             // {
//             //     EnablePlayerBaseFactory = true,
//             //     EnableCardFactory = true,
//             //     EnableGround = true,
//             //     EnableFrontFactory = true,
//             //     EnableRoomUserViewFactory = true,
//             // });
//             // Container.Bind<ICoreGameplay>().FromMock();
//             // Container.Bind<IController>().FromMock();
//             // Container.Bind<IRepository>().FromMock();
//             // yield return null;
//             // var fac2 = Container.Resolve<RoomUserView.IManager>();
//             // var infos = new List<FullUserInfo> {personalInfo};
//             // infos.AddRange(fullUserInfos);
//             // fac2.Init(infos, 0);
//             // var fac = Container.Resolve<PlayerBase.Factory>();
//             // var g = Container.Resolve<IGround>();
//             // var player = (IPlayer) fac.Create(0, 0, 0).ToCoroutine();
//             // var oppo1 = (IOppo) fac.Create(0, 1, 1);
//             // var oppo2 = (IOppo) fac.Create(0, 2, 2);
//             // g.Distribute(new List<int>
//             // {
//             //     0, 1, 2, 3
//             // });
//             // yield return new WaitForSeconds(2);
//             // player.EatLast();
//             // yield return new WaitForSeconds(2);
//             // g.Distribute(new List<int>
//             // {
//             //     0, 1, 2, 3
//             // });
//             // yield return new WaitForSeconds(2);
//             // oppo1.EatLast();
//             // yield return new WaitForSeconds(2);
//             // g.Distribute(new List<int>
//             // {
//             //     0, 1, 2, 3
//             // });
//             // yield return new WaitFoRrSeconds(2);
//             // oppo2.EatLast();
//             // yield return new WaitForSeconds(2);
//         }
//
//         [UnityTest]
//         public IEnumerator ResumeRoom()
//         {
//             yield break;
//             //
//             // InstallProjectModule(new ProjectInstaller.Settings(false)
//             // {
//             //     EnableToast = true,
//             //     EnableBlockingPanel = true,
//             //     EnableBlockingOperationManager = true,
//             //     EnableFullUserView = true,
//             // });
//             // InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             // {
//             //     EnablePlayerBaseFactory = true,
//             //     EnableCardFactory = true,
//             //     EnableGround = true,
//             //     EnableFrontFactory = true,
//             //     EnableRoomUserViewFactory = true,
//             // });
//             // Container.Bind<IController>().FromMock();
//             // Container.Bind<IRepository>().FromMock();
//             // yield return null;
//             // var fac2 = Container.Resolve<RoomUserView.IManager>();
//             // var infos = new List<FullUserInfo> {personalInfo};
//             // infos.AddRange(fullUserInfos);
//             // fac2.Init(infos, 0);
//             // var fac = Container.Resolve<PlayerBase.Factory>();
//             // var g = Container.Resolve<IGround>();
//             // var player = (IPlayer) fac.Create(0, 0);
//             // var oppo1 = (IOppo) fac.Create(1, 1);
//             // var oppo2 = (IOppo) fac.Create(2, 2);
//             // g.Distribute(new List<int>
//             // {
//             //     0, 1, 2, 3
//             // });
//             // yield return new WaitForSeconds(2);
//             // player.EatLast();
//             // yield return new WaitForSeconds(2);
//             // g.Distribute(new List<int>
//             // {
//             //     0, 1, 2, 3
//             // });
//             // yield return new WaitForSeconds(2);
//             // oppo1.EatLast();
//             // yield return new WaitForSeconds(2);
//             // g.Distribute(new List<int>
//             // {
//             //     0, 1, 2, 3
//             // });
//             // yield return new WaitForSeconds(2);
//             // oppo2.EatLast();
//             // yield return new WaitForSeconds(2);
//         }
//
//         [UnityTest]
//         public IEnumerator CreateRoom()
//         {
//             InstallProjectModule(new ProjectInstaller.Settings(false)
//             {
//                 EnableToast = true,
//                 EnableBlockingPanel = true,
//                 EnableBlockingOperationManager = true,
//                 EnableFullUserView = true,
//                 EnableRoomFactory = true,
//                 EnableGlobalBackground = true,
//                 EnableReferenceInstantiator = true,
//                 EnableFinalizeFactory = true,
//                 EnableLobbyFactory = true,
//             });
//
//             //InstallRoomServices(new RoomInstaller.ModuleSwitches(false)
//             //{
//             //    EnablePlayerBaseFactory = true,
//             //    EnableCardFactory = true,
//             //    EnableGround = true,
//             //    EnableFrontFactory = true,
//             //    EnableRoomUserViewFactory = true,
//             //});
//             Container.Bind<IController>().FromMock();
//             Container.Bind<IRepository>().FromMock();
//             yield return null;
//             var fac2 = Container.Resolve<RoomController.Factory>();
//             fac2.Create(new RoomSettings(0, 0, fullUserInfos.ToList(), 0), null);
//         }
//
//       
//     }
// }

