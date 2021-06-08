using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Moq;
using Zenject;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PlayModeTests
{
    [TestFixture]
    public class RoomTests : RoomTestBase
    {
        [SetUp]
        public void Install()
        {
            ProjectContext.Instance.Container.Inject(this);
        }

        [Inject] private ZenjectSceneLoader _zenjectSceneLoader;

        [UnityTest]
        public IEnumerator overallGame()
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

            controller.Setup(_ => _.SendAsync(It.IsAny<string>())).Returns<string, object[]>((method, args) =>
            {
                Debug.Log($"mocking call {method} in the server with args {string.Join(", ", args)}");
                return UniTask.FromResult(new object());
            });

            StaticContext.Container.BindInstance(controller.Object);
            StaticContext.Container.BindInterfacesTo<Repository>().AsSingle();
            var repo = StaticContext.Container.Resolve<IRepository>();
            repo.PersonalFullInfo = personalInfo;
            repo.TopFriends = topFriends;
            repo.YesterdayChampions = yesterdayChampions;

            var opposInfo = new List<RoomOppoInfo>()
            {
                new RoomOppoInfo
                {
                    FullUserInfo = new FullUserInfo
                    {
                        Id = "tstFriendId",
                        Level = 13,
                        DisplayName = "Hany Shaker",
                        Picture = Texture2D.grayTexture,
                        Title = "I am a normal human being"
                    },
                    TurnId = 0
                },
                new RoomOppoInfo
                {
                    FullUserInfo = new FullUserInfo
                    {
                        Id = "tstFriendId2",
                        Level = 93,
                        DisplayName = "Emad Ta3ban",
                        Picture = Texture2D.linearGrayTexture,
                        Title = "abo fa45a"
                    },
                    TurnId = 1
                }
            };


            StaticContext.Container.BindInstance(new RoomSettings(0, 0, opposInfo, 0))
                .WhenInjectedInto<RoomSettings>();

            StaticContext.Container.BindInstance(false);

            _zenjectSceneLoader.LoadScene("Room", extraBindings: container =>
            {
                container.BindInstance(new RoomInstaller.Settings(true)
                {
                    EnablePlayerBaseFactory = true,
                    EnableRoomUserViewFactory = true,
                });
            });

            yield return new WaitForSeconds(999999);
        }

        private DiContainer GetSceneDiContainer()
        {
            return ProjectContext.Instance.Container.Resolve<SceneContextRegistry>()
                .TryGetSceneContextForScene("Room").Container;
        }

        [UnityTest]
        public IEnumerator blank()
        {
            var frontSprites = Addressables.LoadAssetAsync<Sprite[]>("FrontSprites");

            yield return new WaitUntil(() => frontSprites.IsDone);

            Debug.Log(frontSprites.Result.Length);
            Debug.Log(frontSprites.Result[0]);

            yield return new WaitForSeconds(float.MaxValue);
        }


        private void InstallMockRoomController()
        {
            var roomController = new Mock<IRoomController>();
            roomController.Setup(_ => _.CurrentTurn).Returns(0);
            Container.Bind<IRoomController>().FromInstance(roomController.Object);
        }

        PersonalFullUserInfo personalInfo = new PersonalFullUserInfo
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

        MinUserInfo[] topFriends = new[]
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

        MinUserInfo[] yesterdayChampions = new[]
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

        private void InstallMockController()
        {
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

            controller.Setup(_ => _.SendAsync(It.IsAny<string>())).Returns<string, object[]>((method, args) =>
            {
                Debug.Log($"mocking call {method} in the server with args {string.Join(", ", args)}");
                return UniTask.FromResult(new object());
            });

            controller.Setup(_ => _.ThrowCard(It.IsAny<int>()))
                .Returns(UniTask.FromResult(new ThrowResponse {EatenCardsIds = new int[] { }, Basra = true}));

            controller.Setup(_ => _.NotifyTurnMiss()).Callback(() => Debug.Log("notify turn miss called"));

            Container.BindInstance(controller.Object);
        }
        private void InstallMockGround()
        {
            var ground = new Mock<IGround>();
            ground.Setup(_ => _.Throw(It.IsAny<Card>(), It.IsAny<int[]>()))
                .Callback<Card, int[]>((c, arr) => Object.Destroy(c));
            Container.Bind<IGround>().FromMock();
        }

        private void InstallRoomController()
        {
            Container.BindInterfacesTo<RoomController>().AsSingle();
        }

        private void InstallRepository()
        {
            StaticContext.Container.BindInterfacesTo<Repository>().AsSingle();
            var repo = StaticContext.Container.Resolve<IRepository>();
            repo.PersonalFullInfo = personalInfo;
            repo.TopFriends = topFriends;
            repo.YesterdayChampions = yesterdayChampions;
        }


        [UnityTest]
        public IEnumerator PlayerBase_Distribute()
        {
            // var scene = SceneManager.CreateScene("UnitTestScene");
            // _zenjectSceneLoader.LoadScene(scene.name);
            //
            // _zenjectSceneLoader.LoadScene("Room", extraBindings: container =>
            // {
            //     container.BindInstance(new RoomInstaller.Settings(false)
            //     {
            //         EnablePlayerBaseFactory = true,
            //         EnableCardFactory = true,
            //     }).WhenInjectedInto<RoomInstaller>();
            // });

            // yield return null;

            // var sceneContainer = GetSceneDiContainer();

            LoadCore();

            InstallCardFactory();
            InstallFrontFactory();
            InstallPlayerFactory();
            InstallMockRoomController();
            InstallMockController();
            Container.Bind<IGround>().FromMock();

            var fac = Container.Resolve<PlayerBase.Factory>();
            var player = (IPlayer) fac.Create(PlayerType.Me, 1);
            var oppo0 = (IOppo) fac.Create(PlayerType.Oppo, 0);

            player.Distribute(new List<int>() {1, 2, 3, 4});
            oppo0.Distribute();


            yield return new WaitForSeconds(999999);
        }

        [UnityTest]
        public IEnumerator Player_Throw()
        {
            InstallerRoomServices(new RoomInstaller.Settings(false)
            {
                EnableCardFactory = true,
                EnableFrontFactory = true,
                EnablePlayerBaseFactory = true,
                EnableTurnTimer = true,
            });

            InstallMockRoomController();
            InstallMockController();
            InstallGround();

            var fac = Container.Resolve<PlayerBase.Factory>();
            var player = (IPlayer) fac.Create(PlayerType.Me, 0);

            player.Distribute(new List<int>() {1, 2, 3, 4});

            yield return new WaitForSeconds(999999);
        }

        [UnityTest]
        public IEnumerator Oppo_Throw()
        {
            InstallerRoomServices(new RoomInstaller.Settings(false)
            {
                EnableCardFactory = true,
                EnableFrontFactory = true,
                EnablePlayerBaseFactory = true,
            });

            InstallMockRoomController();
            InstallMockController();
            InstallGround();

            var fac = Container.Resolve<PlayerBase.Factory>();
            var oppo = (IOppo) fac.Create(PlayerType.Oppo, 0);

            oppo.Distribute();

            oppo.Throw(0, new ThrowResponse {EatenCardsIds = new int[] { }, BigBasra = true});
            yield return new WaitForSeconds(3);
            oppo.Throw(1, new ThrowResponse {EatenCardsIds = new int[] { }, Basra = true});
            yield return new WaitForSeconds(3);
            oppo.Throw(2, new ThrowResponse {EatenCardsIds = new int[] { }});
            yield return new WaitForSeconds(3);
            oppo.Throw(2, new ThrowResponse {EatenCardsIds = new int[] { }});

            // Assert.Throws(typeof(Exception), () => oppo.Throw(2, new ThrowResponse {EatenCardsIds = new int[] { }}));

            yield return new WaitForSeconds(999999);
        }

        [UnityTest]
        public IEnumerator Ground_Throw()
        {
            LoadCore();

            InstallCardFactory();
            InstallFrontFactory();
            InstallGround();
            InstallMockRoomController();
            InstallMockController();

            var ground = Container.Resolve<IGround>();
            ground.InitialDistribute(new List<int>() {1, 2, 3, 4});

            var cFac = Container.Resolve<Card.Factory>();

            yield return new WaitForSeconds(2);
            ground.Throw(cFac.CreateGroundCard(11, null), new int[] { });
            yield return new WaitForSeconds(2);
            ground.Throw(cFac.CreateGroundCard(11, null), new int[] { });

            yield return new WaitForSeconds(999999);
        }

        [Test]
        public void Ground_Throw_ShouldEatRight()
        {
            LoadCore();

            InstallCardFactory();
            InstallFrontFactory();
            InstallGround();
            InstallMockRoomController();
            InstallMockController();

            var ground = Container.Resolve<IGround>();
            ground.InitialDistribute(new List<int>() {1, 2, 3, 4});

            var cFac = Container.Resolve<Card.Factory>();

            ground.Throw(cFac.CreateGroundCard(11, null), new int[] {1});
            ground.Throw(cFac.CreateGroundCard(11, null), new int[] {2, 3});
            ground.Throw(cFac.CreateGroundCard(11, null), new int[] { });
            ground.Throw(cFac.CreateGroundCard(11, null), new int[] { });

            Assert.Throws(typeof(Exception), () => { ground.Throw(cFac.CreateGroundCard(11, null), new int[] {2}); });

            // yield return new WaitForSeconds(999999);
        }

        [UnityTest]
        public IEnumerator PlayerBase_Turns()
        {
            LoadCore();

            //deps            
            InstallCardFactory();
            InstallFrontFactory();
            Container.Bind<IController>().FromMock();
            Container.Bind<IGround>().FromMock();
            Container.Bind<IRoomController>().FromMock();

            InstallPlayerFactory();
            var pFac = Container.Resolve<PlayerBase.Factory>();
            var p = pFac.Create(PlayerType.Me, 1);
            // p.StartTurn();

            yield return new WaitForSeconds(999999);
        }


        [UnityTest]
        public IEnumerator PlayerBase_AllPlayersDistribute()
        {
            LoadCore();

            InstallTurnTimer();
            InstallCardFactory();
            InstallFrontFactory();
            InstallPlayerFactory();
            InstallMockRoomController();
            InstallMockController();
            Container.Bind<IGround>().FromMock();

            var fac = Container.Resolve<PlayerBase.Factory>();
            var player = (IPlayer) fac.Create(PlayerType.Me, 0);
            var oppo0 = (IOppo) fac.Create(PlayerType.Oppo, 1);
            var oppo1 = (IOppo) fac.Create(PlayerType.Oppo, 2);
            var oppo2 = (IOppo) fac.Create(PlayerType.Oppo, 3);

            player.Distribute(new List<int>() {1, 2, 3, 4});
            oppo0.Distribute();
            oppo1.Distribute();
            oppo2.Distribute();

            yield return new WaitForSeconds(float.MaxValue);
        }

        [UnityTest]
        public IEnumerator TurnTimer_ShouldUpdateAndFireOnComplete()
        {
            InstallerRoomServices(new RoomInstaller.Settings(false)
            {
                EnableTurnTimer = true,
            });
            var tt = Container.Resolve<TurnTimer>();

            yield return null;

            tt.uniTaskTimer.Elapsed += () => Debug.Log("timer is done");

            tt.uniTaskTimer.Play();

            Assert.True(tt.uniTaskTimer.Active);

            yield return new WaitUntil(() => !tt.uniTaskTimer.Active);
        }

        [UnityTest]
        public IEnumerator RoomUserViews_ShouldAllShowInPlaceWithRightData()
        {
            Container.Bind<IRepository>().FromMock();
            Container.Bind<IController>().FromMock();
            InstallerRoomServices(new RoomInstaller.Settings(false)
                {EnableRoomUserViewFactory = true, EnableFullUserView = true, EnablePersonalUserView = true});

            var viewFac = Container.Resolve<RoomUserView.Factory>();

            viewFac.Create(0, personalInfo);
            viewFac.Create(1, topFriends[1]);
            viewFac.Create(2, topFriends[0]);
            viewFac.Create(3, topFriends[1]);

            yield return new WaitForSeconds(float.MaxValue);
        }

        [UnityTest]
        public IEnumerator RoomUserViews_ShouldAllShowFullInfo()
        {
            Container.Bind<IRepository>().FromMock();
            Container.Bind<IController>().FromMock();
            InstallerRoomServices(new RoomInstaller.Settings(false)
                {EnableRoomUserViewFactory = true, EnableFullUserView = true, EnablePersonalUserView = true});

            var viewFac = Container.Resolve<RoomUserView.Factory>();

            viewFac.Create(0, personalInfo);
            viewFac.Create(1, topFriends[1]);
            viewFac.Create(2, topFriends[0]);
            viewFac.Create(3, topFriends[1]);

            yield return new WaitForSeconds(float.MaxValue);
        }
    }
}