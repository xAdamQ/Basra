using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Basra.Models.Client;
using Basra.Server.Exceptions;
using Basra.Server.Extensions;
using Basra.Server.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;


namespace Basra.Server.Services
{
    public interface IRoomManager
    {
        /// <summary>
        /// after ready, change domain, initial distribute, start 0 player turn
        /// </summary>
        Task StartRoom(Room room);

        /// <summary>
        /// when the client is aware that he missed his turn, it sends this message to make
        /// force play faster
        /// </summary>
        Task MissTurnRpc(RoomUser roomUser);

        /// <summary>
        /// is called from timeout
        /// </summary>
        Task ForceUserPlay(RoomUser roomUser);

        /// <summary>
        /// this is the throw function
        /// </summary>
        Task UserPlayRpc(RoomUser roomUser, int cardIndexInHand);

        /// <summary>
        /// is called from timeout
        /// </summary>
        Task BotPlay(RoomBot roomBot);
        Task<ActiveRoomState> GetFullRoomState(RoomUser roomUser);
    }

    /// <summary>
    /// handle active/started room
    /// </summary>
    public class RoomManager : IRoomManager
    {
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly IServerLoop _serverLoop;
        private readonly ILogger<RoomManager> _logger;

        public RoomManager(IHubContext<MasterHub> masterHub, IMasterRepo masterRepo, ISessionRepo sessionRepo,
            IServerLoop serverLoop, ILogger<RoomManager> logger)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
            _serverLoop = serverLoop;
            _logger = logger;
        }


        public async Task StartRoom(Room room)
        {
            room.SetUsersDomains(typeof(UserDomain.App.Room));
            GenerateRoomDeck(room);

            InitialTurn(room);
            await InitialDistribute(room);
        } //no test

        private void GenerateRoomDeck(Room room)
        {
            room.Deck = new List<int>(Enumerable.Range(0, Room.DeckSize));
            room.Deck.Shuffle();
        }

        private void InitialTurn(Room room)
        {
            var roomActor = room.RoomActors[room.CurrentTurn];

            if (roomActor is RoomUser roomUser)
            {
                _serverLoop.SetupTurnTimeout(roomUser);
            }
            else
            {
                _serverLoop.BotPlay(roomActor as RoomBot, StaticRandom.GetRandom(300, 2000));
            }
        }

        private async Task InitialDistribute(Room room)
        {
            room.GroundCards = room.Deck.CutRange(RoomActor.HandSize);

            foreach (var roomActor in room.RoomActors)
                roomActor.Hand = room.Deck.CutRange(RoomActor.HandSize);

            foreach (var roomUser in room.RoomUsers)
                await _masterHub.Clients.User(roomUser.Id)
                    .SendAsync("StartRoomRpc", roomUser.Hand, roomUser.Room.GroundCards);
            //todo check if the user disconnected this will give error or not
        } //the cut part can be tested, but it's relatively easy

        private async Task Distribute(Room room)
        {
            foreach (var roomActor in room.RoomActors)
                roomActor.Hand = roomActor.Room.Deck.CutRange(RoomActor.HandSize);

            // return room.RoomUsers.Select(roomUser => new DistributeResult {MyHand = roomUser.Hand}).ToList();

            foreach (var roomUser in room.RoomUsers)
                await _masterHub.Clients.User(roomUser.Id).SendAsync("Distribute", roomUser.Hand);
        } //trivial to test

        private async Task NextTurn(Room room)
        {
            room.CurrentTurn = ++room.CurrentTurn % room.Capacity;
            var actorInTurn = room.RoomActors[room.CurrentTurn];

            if (actorInTurn.Hand.Count == 0 && actorInTurn.TurnId == 0)
            {
                if (actorInTurn.Room.Deck.Count == 0)
                {
                    await FinalizeGame(actorInTurn.Room);
                    return;
                }
                else
                {
                    await Distribute(actorInTurn.Room);
                }
            }

            if (actorInTurn is RoomUser roomUser)
            {
                if (roomUser.ActiveUser.Disconnected) await ForceUserPlay(roomUser);
                else _serverLoop.SetupTurnTimeout(roomUser);
            }
            else
            {
                // _serverLoop.BotPlay(actorInTurn as RoomBot, StaticRandom.GetRandom(300, 2000));
                _serverLoop.BotPlay(actorInTurn as RoomBot, StaticRandom.GetRandom(99999999, 999999999));
                //this is correct because sever loop is singleton not scoped
            }
        }

        private ThrowResult PlayBase(RoomActor roomActor, int cardIndexInHand)
        {
            var eaten = Eat(roomActor.Hand[cardIndexInHand], roomActor.Room.GroundCards, out bool basra,
                out bool bigBasra);

            var card = roomActor.Hand.Cut(cardIndexInHand);

            if (eaten.Count != 0)
            {
                roomActor.Room.GroundCards.RemoveAll(c => eaten.Contains(c));

                roomActor.EatenCardsCount += eaten.Count;
                if (basra) roomActor.BasraCount++;
                if (bigBasra) roomActor.BigBasraCount++;
            }
            else
            {
                roomActor.Room.GroundCards.Add(card);
            }

            return new ThrowResult
            {
                ThrownCard = card,
                Basra = basra,
                BigBasra = bigBasra,
                EatenCardsIds = eaten,
            };
        }

        public async Task UserPlayRpc(RoomUser roomUser, int cardIndexInHand)
        {
            if (roomUser.TurnId != roomUser.Room.CurrentTurn || !cardIndexInHand.IsInRange(roomUser.Hand.Count))
                throw new BadUserInputException();
            //this is invoked by the server also, and may be a server error and it's handle way is ignoring and
            //terminate the action hub exc are not handled when the actor is the system

            await UserPlay(roomUser, cardIndexInHand);
        }
        private async Task UserPlay(RoomUser roomUser, int cardIndexInHand)
        {
            _serverLoop.CancelTurnTimeout(roomUser);

            var throwResult = PlayBase(roomUser, cardIndexInHand);

            await Task.WhenAll(
                _masterHub.Clients.User(roomUser.Id).SendAsync("MyThrowResult", throwResult),
                _masterHub.Clients.Users(roomUser.Room.RoomUsers.Where(ru => ru != roomUser)
                        .Select(ru => ru.Id))
                    .SendAsync("CurrentOppoThrow", throwResult)
            );

            await NextTurn(roomUser.Room);
            _logger.LogInformation($"user has played card {cardIndexInHand} userId {roomUser.Id}");
        } //todo good candidate for unit testing

        public async Task MissTurnRpc(RoomUser roomUser) //the difference is that rpc contains validation
        {
            if (roomUser.TurnId != roomUser.Room.CurrentTurn)
                //this check is done by the domain, but i think it should be done like this because we already have the turn stuff here
                //so we don't maintain in both

                throw new BadUserInputException();

            await RandomUserPlay(roomUser, notification: true);
        }

        public async Task ForceUserPlay(RoomUser roomUser)
        {
            await RandomUserPlay(roomUser, notification: false);
        }

        /// <param name="notification">
        /// if client side send he missed his turn, or if it happened because of internal timout
        /// </param>
        private async Task RandomUserPlay(RoomUser roomUser, bool notification)
        {
            var randomCardIndex = StaticRandom.GetRandom(roomUser.Hand.Count);

            // await UserPlay(roomUser, randomCardIndex);

            if (notification)
                _serverLoop.CancelTurnTimeout(roomUser);

            var throwResult = PlayBase(roomUser, randomCardIndex);

            var tasks = new List<Task>();

            if (!roomUser.ActiveUser.Disconnected)
                tasks.Add(_masterHub.Clients.User(roomUser.Id)
                    .SendAsync("ForcePlay", throwResult));

            //todo then you have to do the same assertion on this!
            tasks.Add(_masterHub.Clients.Users(roomUser.Room.RoomUsers.Where(ru => ru != roomUser).Select(ru => ru.Id))
                .SendAsync("CurrentOppoThrow", throwResult));

            await Task.WhenAll(tasks);

            await NextTurn(roomUser.Room);
        }
        //no test, you can test the "concurrent random"

        public async Task BotPlay(RoomBot roomBot)
        {
            //can happen with logic
            var randomCardIndex = StaticRandom.GetRandom(roomBot.Hand.Count);

            var throwResult = PlayBase(roomBot, randomCardIndex);

            await _masterHub.Clients
                .Users(roomBot.Room.RoomUsers.Select(ru =>
                    ru.Id)) //send to all room users, no exception because you're a bot
                .SendAsync("CurrentOppoThrow", throwResult);

            await NextTurn(roomBot.Room);
            _logger.LogInformation($"bot {roomBot.Id} has played card {randomCardIndex}");
        } //no test, you can test the "concurrent random"

        private const int KOMI_ID = 19, BOY_VALUE = 11;
        private static readonly int[] BOY_IDS = { 10, 23, 36, 49 };
        private static List<int> Eat(int cardId, List<int> ground, out bool basra, out bool bigBasra)
        {
            basra = false;
            bigBasra = false;

            var cardValue = cardValueFromId(cardId);

            if (cardId == KOMI_ID)
            {
                basra = ground.Count == 1;

                return ground.ToList();
            }

            if (cardValue == BOY_VALUE)
            {
                bigBasra = ground.Count == 1 && BOY_IDS.Contains(ground[0]);

                return ground.ToList();
            }
            if (cardValue > 10)
            {
                return ground.Where(c => cardValueFromId(c) == cardValue).ToList();
            }

            var groups = ground.Permutations();
            var bestGroupLength = -1;
            var bestGroup = new List<int>();
            foreach (var group in groups)
            {
                if (group.Select(c => cardValueFromId(c)).Sum() == cardValue && group.Count > bestGroupLength)
                {
                    bestGroup = group;
                    bestGroupLength = bestGroup.Count;
                }
            }

            return bestGroup;

            int cardValueFromId(int id)
            {
                return (id % 13) + 1;
            }
        } //tested

        public async Task Surrender(RoomUser roomUser)
        {
            var room = roomUser.Room;

            var currentActor = room.RoomActors[room.CurrentTurn];
            if (currentActor is RoomUser ru)
                _serverLoop.CancelTurnTimeout(ru);

            var otherUsers = room.RoomUsers.Where(ru => ru != roomUser);
            await Task.WhenAll(otherUsers.Select(u =>
                _masterHub.Clients.User(u.Id).SendAsync("UserSurrender", roomUser.TurnId)));
            //blocks the client and waits for finalize result

            await FinalizeGame(roomUser.Room, roomUser);
        }

        private async Task FinalizeGame(Room room, RoomUser resignedUser = null)
        {
            room.SetUsersDomains(typeof(UserDomain.App.Room.FinishedRoom));

            var roomDataUsers = await _masterRepo.GetUsersByIds(room.RoomActors.Select(_ => _.Id).ToList());
            //todo test if the result is the same order as roomUsers

            var scores = CalcScores(room.RoomActors, resignedUser);
            var xpReports = UpdateUserStates(room, roomDataUsers, scores);
            await Task.WhenAll(roomDataUsers.Select(u => LevelWorks(u)));

            await _masterRepo.SaveChangesAsync();

            await SendFinalizeResult(room, roomDataUsers, xpReports);

            RemoveDisconnectedUsers(room.RoomUsers);

            room.RoomUsers.ForEach(ru => _sessionRepo.DeleteRoomUser(ru));
            _sessionRepo.DeleteRoom(room);

            // return GetFinalizeResult(roomDataUsers, xpReports);
        } //todo better split before test 

        /// <summary>
        /// score for resigned user is -1
        /// </summary> 
        private List<int> CalcScores(List<RoomActor> roomActors, RoomUser resignedUser = null)
        {
            var lastedUsers = resignedUser == null ? roomActors : roomActors.Where(_ => _ != resignedUser);

            var biggestEatenCount = lastedUsers.Max(u => u.EatenCardsCount);
            var biggestEaters = lastedUsers.Where(u => u.EatenCardsCount == biggestEatenCount).ToArray();

            var scores = new List<int>();

            foreach (var roomActor in roomActors)
            {
                if (roomActor == resignedUser)
                {
                    scores.Add(-1);
                    continue;
                }

                scores.Add(roomActor.BasraCount * 10 +
                           roomActor.BigBasraCount * 30 +
                           (biggestEaters.Contains(roomActor) ? 30 : 0));
            }

            return scores;
        }

        /// <returns> the added xp and for what </returns>
        private RoomXpReport[] UpdateUserStates(Room room, List<User> dataUsers, List<int> scores)
        {
            var xpReports = new RoomXpReport[room.Capacity];
            for (int i = 0; i < xpReports.Length; i++) xpReports[i] = new RoomXpReport();

            var betWithoutTicket = (int)(room.Bet / 1.1f);
            var totalBet = betWithoutTicket * room.Capacity;
            var maxScore = scores.Max();
            var betXp = CalcBetXp(room.BetChoice);

            var winnerIndices = scores.Select((score, i) => score == maxScore ? i : -1)
                .Where(scoreIndex => scoreIndex != -1)
                .ToList();
            var loserIndices = Enumerable.Range(0, room.Capacity)
                .Where(i => !winnerIndices.Contains(i))
                .ToList();
            var resignedUserIndex = scores.IndexOf(-1); //if no resign it retrun -1

            //drawers
            if (winnerIndices.Count > 1)
            {
                var moneyPart = totalBet / winnerIndices.Count;
                foreach (var userIndex in winnerIndices)
                {
                    var dUser = dataUsers[userIndex];
                    dUser.Draws++;
                    dUser.Money += moneyPart;
                    dUser.TotalEarnedMoney += moneyPart;

                    dUser.XP += xpReports[userIndex].Competition = (int)Room.DrawXpPercent * betXp;
                }
            }
            //winner
            else
            {
                var dUser = dataUsers[winnerIndices[0]];
                dUser.WonRoomsCount++;
                dUser.Money += totalBet;
                dUser.TotalEarnedMoney += totalBet;
                dUser.WinStreak++;

                dUser.XP += xpReports[0].Competition = (int)Room.WinXpPercent * betXp;
            }


            //losers
            foreach (var loserIndex in loserIndices)
            {
                var dUser = dataUsers[loserIndex];
                if (loserIndex != resignedUserIndex)
                    dUser.XP += xpReports[loserIndex].Competition = (int)Room.LoseXpPercent * betXp;

                dUser.WinStreak = 0;
            }

            for (int i = 0; i < room.Capacity; i++)
            {
                var dUser = dataUsers[i];
                var roomActor = room.RoomActors[i];

                dUser.PlayedRoomsCount++;

                dUser.EatenCardsCount += roomActor.EatenCardsCount;
                dUser.BasraCount += roomActor.BasraCount;
                dUser.BigBasraCount += roomActor.BigBasraCount;

                if (i != resignedUserIndex)
                {
                    dUser.XP += xpReports[i].Basra = roomActor.BasraCount * (int)Room.BasraXpPercent * betXp;
                    dUser.XP += xpReports[i].BigBasra = roomActor.BigBasraCount * (int)Room.BigBasraXpPercent * betXp;

                    if (roomActor.EatenCardsCount > Room.GreatEatThreshold)
                        dataUsers[i].XP += xpReports[i].GreatEat = (int)Room.GreatEatXpPercent * betXp;
                }
            }

            return xpReports;
        }

        private int CalcBetXp(int betChoice) => (int)(100 * MathF.Pow(betChoice, 1.4f)) + 100;

        private void RemoveDisconnectedUsers(List<RoomUser> roomUsers)
        {
            foreach (var roomUser in roomUsers.Where(ru => ru.ActiveUser.Disconnected))
                //where filtered with new collection, I don't know the performance but I will see how
                //linq works under the hood, because I think the created collection doesn't affect performance
                _sessionRepo.RemoveActiveUser(roomUser.ActiveUser.Id);
        }

        /// <summary>
        /// check current level againest xp to level up and send to client
        /// functions that takes data user as param dosn't save changes
        /// </summary>
        private async Task LevelWorks(User roomDataUser) //separate this to be called on every XP change 
        {
            var calcedLevel = Room.GetLevelFromXp(roomDataUser.XP);
            if (calcedLevel > roomDataUser.Level)
            {
                var increasedLevels = calcedLevel - roomDataUser.Level;
                var totalMoneyReward = 0;
                for (int j = 0; j < increasedLevels; j++)
                {
                    totalMoneyReward += 100;
                    //todo give level up rewards (money equation), add to test
                    //todo test this function logic
                }

                roomDataUser.Level = calcedLevel;
                roomDataUser.Money = totalMoneyReward;

                await _masterHub.Clients.User(roomDataUser.Id)
                    .SendAsync("LevelUp", calcedLevel, totalMoneyReward);
            }
        }

        private async Task SendFinalizeResult(Room room, List<User> roomDataUsers, RoomXpReport[] roomXpReports)
        {
            var updateProfileTasks = new List<Task>();
            for (int i = 0; i < room.RoomUsers.Count; i++)
            {
                var finalizeResult = new FinalizeResult
                {
                    RoomXpReport = roomXpReports[i],
                    PersonalFullUserInfo = Mapper.ConvertUserDataToClient(roomDataUsers[i]),
                };

                updateProfileTasks.Add(_masterHub.Clients.User(room.RoomUsers[i].Id)
                    .SendAsync("FinalizeResult", finalizeResult));
            }
            await Task.WhenAll(updateProfileTasks);
        }

        public async Task<ActiveRoomState> GetFullRoomState(RoomUser roomUser)
        {
            var room = roomUser.Room;
            var usersInfo = await _masterRepo.GetFullUserInfoListAsync(room.RoomActors.Select(ra => ra.Id));

            var roomState = new ActiveRoomState
            {
                CurrentTurn = room.CurrentTurn,
                FullUsersInfo = usersInfo,
                Ground = room.GroundCards,
                MyHand = roomUser.Hand,
                TurnId = roomUser.TurnId,

                OppoHandCounts = new List<int>(),
                BasrasCounts = new List<int>(),
                BigBasrasCounts = new List<int>(),
                EatenCardsCounts = new List<int>(),
            };

            room.RoomActors.ForEach(ra =>
            {
                roomState.OppoHandCounts.Add(ra.Hand.Count);
                roomState.BasrasCounts.Add(ra.BasraCount);
                roomState.BigBasrasCounts.Add(ra.BigBasraCount);
                roomState.EatenCardsCounts.Add(ra.EatenCardsCount);
            });

            return roomState;
        }
    }
}