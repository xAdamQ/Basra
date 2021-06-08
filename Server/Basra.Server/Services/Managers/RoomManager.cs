using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
            room.RoomUsers.ForEach(ru => ru.ActiveUser.Domain = typeof(UserDomain.App.Room));
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
                    .SendAsync("InitialDistribute", roomUser.Hand, roomUser.Room.GroundCards);
            //todo check if the user disconnected this will give error or not
        } //the cut part can be tested, but it's relatively easy

        private async Task Distribute(Room room)
        {
            foreach (var roomActor in room.RoomActors)
                roomActor.Hand = roomActor.Room.Deck.CutRange(RoomActor.HandSize);

            foreach (var roomUser in room.RoomUsers)
                await _masterHub.Clients.User(roomUser.Id).SendAsync("Distribute", roomUser.Hand);
        } //trivial to test

        private async Task NextTurn(Room room)
        {
            room.CurrentTurn = ++room.CurrentTurn % room.Capacity;
            var roomActor = room.RoomActors[room.CurrentTurn];

            if (roomActor is RoomUser roomUser)
            {
                _serverLoop.SetupTurnTimeout(roomUser);
            }
            else
            {
                _serverLoop.BotPlay(roomActor as RoomBot, StaticRandom.GetRandom(300, 2000));
                //this is correct because sever loop is singleton not scoped
            }

            if (roomActor.Hand.Count == 0 && roomActor.TurnId == 0)
                if (roomActor.Room.Deck.Count == 0)
                    await FinalizeGame(roomActor.Room);
                else
                    await Distribute(roomActor.Room);
        }

        private (int, List<int>) PlayBase(RoomActor roomActor, int cardIndexInHand)
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

            return (card, eaten);
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

            var cardAndEaten = PlayBase(roomUser, cardIndexInHand);

            await Task.WhenAll(
                _masterHub.Clients.User(roomUser.Id).SendAsync("MyThrowResult", cardAndEaten.Item2),
                _masterHub.Clients.Users(roomUser.Room.RoomUsers.Where(ru => ru != roomUser)
                        .Select(ru => ru.Id))
                    .SendAsync("CurrentOppoThrow", cardAndEaten.Item1, cardAndEaten.Item2)
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

            var cardAndEaten = PlayBase(roomUser, randomCardIndex);

            var tasks = new List<Task>();

            if (!roomUser.ActiveUser.Disconnected)
                tasks.Add(_masterHub.Clients.User(roomUser.Id)
                    .SendAsync("ForcePlay", randomCardIndex, cardAndEaten.Item2));

            //todo then you have to do the same assertion on this!
            tasks.Add(_masterHub.Clients.Users(roomUser.Room.RoomUsers.Where(ru => ru != roomUser).Select(ru => ru.Id))
                .SendAsync("CurrentOppoThrow", cardAndEaten.Item1, cardAndEaten.Item2));

            await Task.WhenAll(tasks);

            await NextTurn(roomUser.Room);
        }
        //no test, you can test the "concurrent random"

        public async Task BotPlay(RoomBot roomBot)
        {
            //can happen with logic
            var randomCardIndex = StaticRandom.GetRandom(roomBot.Hand.Count);

            var cardAndEaten = PlayBase(roomBot, randomCardIndex);

            await _masterHub.Clients
                .Users(roomBot.Room.RoomUsers.Select(ru =>
                    ru.Id)) //send to all room users, no exception because you're a bot
                .SendAsync("CurrentOppoThrow", cardAndEaten.Item1, cardAndEaten.Item2);

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

            roomUser.Resigned = true;

            var otherUsers = room.RoomUsers.Where(ru => ru != roomUser);
            await Task.WhenAll(otherUsers.Select(u => _masterHub.Clients.User(u.Id).SendAsync("UserSurrender", roomUser.TurnId)));
            //blocks the client and waits for finalize result

            await FinalizeGame(roomUser.Room);
        }


        private async Task FinalizeGame(Room room)
        {
            _sessionRepo.DeleteRoom(room);


            // var lastedActors = resginedUser != null ?
            // room.RoomActors.Where(ra => ra != resginedUser).ToList() :
            // room.RoomActors;

            var roomDataUsers = await _masterRepo.GetUsersByIds(room.RoomActors.Select(_ => _.Id).ToList());
            //todo test if the result is the same order as roomUsers

            await CalcAndApplyGameResults(room, roomDataUsers);

            await _masterRepo.SaveChangesAsync();

            await SendUserInfo(room, roomDataUsers);

            MakeUsersDomainFinishedRoom(room.RoomUsers);

            RemoveDisconnectedUsers(room.RoomUsers);
        } //todo better split before test 

        /// <summary>
        /// socre for resigned user is always 0
        /// </summary> 
        private List<int> CalcScores(List<RoomActor> roomActors)
        {
            var lastedUsers = roomActors.Where(_ => (_ is RoomUser ru && !ru.Resigned) || _ is not RoomUser);

            var biggestEatenCount = lastedUsers.Max(u => u.EatenCardsCount);
            var biggestEaters = lastedUsers.Where(u => u.EatenCardsCount == biggestEatenCount).ToArray();

            var scores = new List<int>();

            foreach (var roomActor in roomActors)
            {
                if (roomActor is RoomUser ru && ru.Resigned)
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
        private async Task CalcAndApplyGameResults(Room room, List<User> roomDataUsers)//todo split this
        {
            var scores = CalcScores(room.RoomActors);
            var totalBet = room.Bet * room.Capacity;
            var maxScore = scores.Max();
            var betXp = CalcBetXp(room.BetChoice);

            // var winners = scores.Where(_ => _.Value == maxScore).Select(_ => _.Key).ToList();
            // var losers = scores.Keys.Except(winners);

            var winnerIndices = scores.Select((score, i) => score == maxScore ? i : -1)
                .Where(scoreIndex => scoreIndex != -1)
                .ToList();
            var loserIndices = Enumerable.Range(0, room.Capacity)
                .Where(i => !winnerIndices.Contains(i))
                .ToList();

            //drawers
            if (winnerIndices.Count > 1)
            {
                var moneyPart = totalBet / winnerIndices.Count;
                foreach (var userIndex in winnerIndices)
                {
                    var dUser = roomDataUsers[userIndex];
                    dUser.Draws++;
                    dUser.Money += moneyPart;
                    dUser.TotalEarnedMoney += moneyPart;
                    dUser.XP += (int)Room.DrawXpPercent * betXp;
                }
            }
            //winner
            else
            {
                var dUser = roomDataUsers[winnerIndices[0]];
                dUser.WonRoomsCount++;
                dUser.Money += totalBet;
                dUser.TotalEarnedMoney += totalBet;
                dUser.XP += (int)Room.WinXpPercent * betXp;
                dUser.WinStreak++;
            }

            //losers
            foreach (var loserIndex in loserIndices)
            {
                var dUser = roomDataUsers[loserIndex];
                dUser.XP += (int)Room.LoseXpPercent * betXp;
                dUser.WinStreak = 0;
            }

            //this part of good deeds system
            //todo more work on complete deeds system
            //add in game great things xp like eating alot, basra with many cards, etc..
            for (int i = 0; i < room.Capacity; i++)
            {
                roomDataUsers[i].PlayedRoomsCount++;

                roomDataUsers[i].EatenCardsCount += room.RoomUsers[i].EatenCardsCount;
                roomDataUsers[i].BasraCount += room.RoomUsers[i].BasraCount;
                roomDataUsers[i].BigBasraCount += room.RoomUsers[i].BigBasraCount;

                roomDataUsers[i].XP += (int)(room.RoomUsers[i].BasraCount * Room.BasraXpPercent * betXp);
                roomDataUsers[i].XP += (int)(room.RoomUsers[i].BigBasraCount * Room.BigBasraXpPercent * betXp);

                await LevelWorks(roomDataUsers[i]); //this uses xp and should be invoked after all xp edits
            }
        }
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
                    //todo give level up rewards (money equation)
                    //todo test this function logic
                }

                roomDataUser.Level = calcedLevel;
                roomDataUser.Money = totalMoneyReward;

                await _masterHub.Clients.User(roomDataUser.Id)
                    .SendAsync("LevelUp", calcedLevel, totalMoneyReward);
            }
        }
        private int CalcBetXp(int betChoice) => (int)(100 * MathF.Pow(betChoice, 1.4f)) + 100;
        private async Task SendUserInfo(Room room, List<User> roomDataUsers)
        {
            var updateProfileTasks = new List<Task>();
            for (int i = 0; i < room.Capacity; i++)
            {
                updateProfileTasks.Add(_masterHub.Clients.User(room.RoomUsers[i].Id).SendAsync("UpdatePersonalInfo",
                    Mapper.ConvertUserDataToClient(roomDataUsers[i])));
            }
            await Task.WhenAll(updateProfileTasks);
        }
        private void MakeUsersDomainFinishedRoom(List<RoomUser> roomUsers)
        {
            roomUsers.ForEach(ru => ru.ActiveUser.Domain = typeof(UserDomain.App.Room.FinishedRoom));
        }
        private void RemoveDisconnectedUsers(List<RoomUser> roomUsers)
        {
            foreach (var roomUser in roomUsers.Where(ru => ru.ActiveUser.Disconnected))
                //where filtered with new collection, I don't know the performance but I will see how
                //linq works under the hood, because I think the created collection doesn't affect performance
                _sessionRepo.RemoveActiveUser(roomUser.ActiveUser.Id);
        }
    }
}