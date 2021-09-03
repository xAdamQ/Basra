using Basra.Common;
using Basra.Server.Exceptions;
using Basra.Server.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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
        Task ShowMessage(RoomUser roomUser, string msgId);
    }

    /// <summary>
    /// handle active/started room
    /// </summary>
    public class RoomManager : IRoomManager
    {
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IMasterRepo _masterRepo;
        private readonly IServerLoop _serverLoop;
        private readonly ILogger<RoomManager> _logger;
        private readonly IFinalizeManager _finalizeManager;

        public RoomManager(IHubContext<MasterHub> masterHub, IMasterRepo masterRepo,
            IServerLoop serverLoop, ILogger<RoomManager> logger, IFinalizeManager finalizeManager)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _serverLoop = serverLoop;
            _logger = logger;
            _finalizeManager = finalizeManager;
        }

        public async Task StartRoom(Room room)
        {
            if (room.Started)
            {
                _logger.LogWarning("the start room is called twice!");
                return;
            }

            room.Started = true;

            room.SetUsersDomains(typeof(UserDomain.App.Room.Active));
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
                _serverLoop.BotPlay(roomActor as RoomBot);
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

            var callName = room.Deck.Count > 0 ? "Distribute" : "LastDistribute";

            foreach (var roomUser in room.RoomUsers)
                await _masterHub.Clients.User(roomUser.Id).SendAsync(callName, roomUser.Hand);
        } //trivial to test

        private async Task NextTurn(Room room)
        {
            room.CurrentTurn = ++room.CurrentTurn % room.Capacity;
            var actorInTurn = room.RoomActors[room.CurrentTurn];

            if (actorInTurn.Hand.Count == 0)
            {
                if (actorInTurn.Room.Deck.Count == 0)
                {
                    await _finalizeManager.FinalizeRoom(actorInTurn.Room);
                    return;
                }
                else
                {
                    await Distribute(actorInTurn.Room);
                }
            }

            if (actorInTurn is RoomUser roomUser)
            {
                if (roomUser.ActiveUser.IsDisconnected) await ForceUserPlay(roomUser);
                else _serverLoop.SetupTurnTimeout(roomUser);
            }
            else
            {
                _serverLoop.BotPlay(actorInTurn as RoomBot);
                //this is correct because sever loop is singleton not scoped
            }
        }

        private ThrowResult PlayBase(RoomActor roomActor, int cardIndexInHand)
        {
            var eaten = Eat(roomActor.Hand[cardIndexInHand], roomActor.Room.GroundCards,
                out bool basra,
                out bool bigBasra);

            var card = roomActor.Hand.Cut(cardIndexInHand);

            if (eaten != null && eaten.Count != 0)
            {
                roomActor.Room.LastEater = roomActor;

                roomActor.Room.GroundCards.RemoveAll(c => eaten.Contains(c));

                roomActor.EatenCardsCount += eaten.Count + 1; //1 is my card
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
            if (roomUser.TurnId != roomUser.Room.CurrentTurn ||
                !cardIndexInHand.IsInRange(roomUser.Hand.Count))
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

            //todo possible issue: one of the clients takes the the message, the other is experiencing network issue
            //then next turn won't be called while we are waiting for that user, and the fast user can make action and lead to exc 

            _logger.LogInformation(
                $"user has played card {cardIndexInHand} with value {throwResult.ThrownCard} userId {roomUser.Id}");

            await NextTurn(roomUser.Room);
        }

        public async Task
            MissTurnRpc(RoomUser roomUser) //the difference is that rpc contains validation
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

            if (!roomUser.ActiveUser.IsDisconnected)
                tasks.Add(_masterHub.Clients.User(roomUser.Id)
                    .SendAsync("ForcePlay", throwResult));

            //todo then you have to do the same assertion on this!
            tasks.Add(_masterHub.Clients
                .Users(roomUser.Room.RoomUsers.Where(ru => ru != roomUser).Select(ru => ru.Id))
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
        public static List<int> Eat(int cardId, List<int> ground, out bool basra,
            out bool bigBasra)
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
                bigBasra = ground.TrueForAll(c => cardValueFromId(c) == BOY_VALUE);

                return ground.ToList();
            }
            if (cardValue > 10)
            {
                var eaten = ground.Where(c => cardValueFromId(c) == cardValue).ToList();
                basra = eaten.Count != 0 && eaten.Count == ground.Count;
                return eaten;
            }

            var groups = ground.Permutations();
            var bestGroupLength = -1;
            List<int> bestGroup = new List<int>();
            foreach (var group in groups)
            {
                if (group.Select(c => cardValueFromId(c)).Sum() == cardValue &&
                    group.Count > bestGroupLength)
                {
                    bestGroup = group;
                    bestGroupLength = bestGroup.Count;
                }
            }

            //since you're here
            basra = bestGroup.Count != 0 && bestGroup.Count == ground.Count;

            return bestGroup;

            int cardValueFromId(int id)
            {
                return (id % 13) + 1;
            }
        } //tested

        // public async Task Surrender(RoomUser roomUser)(RoomUser roomUser)
        // {
        //     var room = roomUser.Room;

        //     var currentActor = room.RoomActors[room.CurrentTurn];
        //     if (currentActor is RoomUser ru)
        //         _serverLoop.CancelTurnTimeout(ru);

        //     var otherUsers = room.RoomUsers.Where(ru => ru != roomUser);
        //     await Task.WhenAll(otherUsers.Select(u =>
        //         _masterHub.Clients.User(u.Id).SendAsync("UserSurrender", roomUser.TurnId)));
        //     //blocks the client and waits for finalize result

        //     // room.RoomActors.First(_ => _ == roomUser);

        //     // if(room)
        //     await _finalizeManager.FinalizeRoom(roomUser.Room, roomUser);
        // }


        private static readonly HashSet<string> EmojiIds = new()
        {
            "angle",
            "angry",
            "dead",
            "cry",
            "devil",
            "heart",
            "cat1",
            "cat2",
            "cat3",
            "moon",
            "mindBlow",
            "bigEye",
            "frog",
            "laughCry",
        };
        private static readonly HashSet<string> TextIds = new()
        {
            "soLucky",
            "comeAgain",
            "congrates",
            "tough",
            "kofta",
            "anyWords",
            "kossa",
        };

        public async Task ShowMessage(RoomUser roomUser, string msgId)
        {
            if (!EmojiIds.Contains(msgId) && !TextIds.Contains(msgId))
                throw new BadUserInputException("message Id is not valid");

            var oppoIds = roomUser.Room.RoomUsers.Where(u => u != roomUser).Select(u => u.Id);
            await _masterHub.Clients.Users(oppoIds)
                .SendAsync("ShowMessage", roomUser.TurnId, msgId);
        }


        // public async Task<ActiveRoomState> GetFullRoomState(RoomUser roomUser)
        // {
        //     var room = roomUser.Room;
        //
        //     var opposInfo = await _masterRepo.GetFullUserInfoListAsync(
        //         room.RoomActors.Where(ra => ra != roomUser).Select(ra => ra.Id));
        //     //get users info from db
        //
        //
        //     var oppoRoomData = opposInfo.Join(room.RoomActors, info => info.Id, actor => actor.Id,
        //         (_, actor) => new {actor.TurnId, actor.Hand.Count}).ToList();
        //
        //     var roomState = new ActiveRoomState
        //     {
        //         BetChoice = room.BetChoice,
        //         CapacityChoice = room.CapacityChoice,
        //         CurrentTurn = room.CurrentTurn,
        //         Ground = room.GroundCards,
        //
        //         OpposInfo = opposInfo,
        //
        //         OpposTurnIds = oppoRoomData.Select(_ => _.TurnId).ToList(),
        //         OppoHandCounts = oppoRoomData.Select(_ => _.Count).ToList(),
        //
        //         MyHand = roomUser.Hand,
        //         MyTurnId = roomUser.TurnId,
        //     };
        //
        //     return roomState;
        // }

        public async Task<ActiveRoomState> GetFullRoomState(RoomUser roomUser)
        {
            var room = roomUser.Room;

            var userData = await _masterRepo.GetFullUserInfoListAsync(room.RoomActorIds);

            var turnSortedInfo = room.RoomActors.Join(userData, actor => actor.Id, info => info.Id,
                (_, info) => info).ToList();

            var roomState = new ActiveRoomState
            {
                BetChoice = room.BetChoice,
                CapacityChoice = room.CapacityChoice,
                CurrentTurn = room.CurrentTurn,
                Ground = room.GroundCards,

                UserInfos = turnSortedInfo,

                HandCounts = room.RoomActors.Select(_ => _.Hand.Count).ToList(),

                MyHand = roomUser.Hand,
                MyTurnId = roomUser.TurnId,

                LastHand = room.Deck.Count == 0,
            };

            return roomState;
        }
    }
}