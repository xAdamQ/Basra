using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Data;
using Basra.Server.Exceptions;
using Basra.Server.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Basra.Server.Services
{
    public interface IRoomManager
    {
        Task RequestRoom(int genre, int betChoice, int capacityChoice, string userId, string connId);
        Task FinalizeGame(Room room);

        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        Task Ready(RoomUser roomUser);

        Task RandomPlay(RoomUser roomUser);
        Task Play(RoomUser roomUser, int cardIndexInHand);
    }

    public class RoomManager : IRoomManager
    {
        private readonly ILogger _logger;
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly IServerLoop _serverLoop;

        public RoomManager(ILogger<RoomManager> logger, IHubContext<MasterHub> masterHub, IMasterRepo masterRepo,
            ISessionRepo sessionRepo,
            IServerLoop serverLoop)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
            _serverLoop = serverLoop;
            _logger = logger;
        }

        //thread safe
        //trivial to test
        public async Task RequestRoom(int genre, int betChoice, int capacityChoice, string userId, string connId)
        {
            var room = _sessionRepo.GetPendingRoom(betChoice, capacityChoice);
            if (room == null)
            {
                room = _sessionRepo.MakeRoom(betChoice, capacityChoice);
                _logger.LogInformation("a new room is made");
            }

            await AddUser(room, userId, connId);
        }

        //trivial logic to test
        private async Task AddUser(Room room, string userId, string connId)
        {
            var rUser = _sessionRepo.AddRoomUser(userId, connId, room);

            if (room.Capacity == room.RoomUsers.Count)
            {
                _logger.LogInformation("a room is ready and will start");
                await StartRoom(room);
            }
            else
            {
                _sessionRepo.KeepRoom(room);
                await _masterHub.Clients.User(rUser.UserId).SendAsync("RoomIsFilling");
            }
        }

        private async Task StartRoom(Room room)
        {
            room.Deck = GenerateDeck();

            List<int> GenerateDeck()
            {
                var deck = new List<int>();
                for (int i = 0; i < Room.DeckSize; i++)
                {
                    deck.Add(i);
                }

                deck.Shuffle();
                return deck;
            }

            room.GroundCards = room.Deck.CutRange(RoomUser.HandSize);

            var dUsers = _masterRepo.GetRoomDisplayUsersAsync(room);
            var rUsers = room.RoomUsers;

            var tasks = new List<Task>();
            for (int i = 0; i < room.Capacity; i++)
            {
                rUsers[i].TurnId = i;

                tasks.Add(_masterHub.Groups.AddToGroupAsync(rUsers[i].ConnectionId, "room" + room.Id));
                tasks.Add(_masterHub.Clients.User(rUsers[i].UserId).SendAsync("StartRoom", i, dUsers));
            }

            await Task.WhenAll(tasks);
        }

        //todo logic to test
        public async Task FinalizeGame(Room room)
        {
            _sessionRepo.DeleteRoom(room);

            var biggestEatenCount = room.RoomUsers.Max(u => u.EatenCardsCount);
            var biggestEaters = room.RoomUsers.Where(u => u.EatenCardsCount == biggestEatenCount).ToArray();

            for (int u = 0; u < room.Capacity; u++)
            {
                room.RoomUsers[u].Score =
                    room.RoomUsers[u].BasraCount * 10 +
                    room.RoomUsers[u].BigBasraCount * 30 +
                    (biggestEaters.Contains(room.RoomUsers[u]) ? 30 : 0);
            }

            var maxScore = room.RoomUsers.Max(u => u.Score);
            var winners = room.RoomUsers.Where(u => u.Score == maxScore).ToArray();
            var totalBet = Room.GenreBets[room.Bet] * room.Capacity;

            //draw
            if (winners.Length > 1)
            {
                var moneyPart = totalBet / winners.Length;
                foreach (var user in winners)
                {
                    //var sUser = user.ActiveUser.Data;
                    var dUser = await _masterRepo.GetUserByIdAsyc(user.UserId);
                    dUser.PlayedGames++;
                    dUser.Draws++;
                    dUser.Money += moneyPart;
                }
            }
            //win
            else
            {
                //var sUser = winners[0].Data;
                var dUser = await _masterRepo.GetUserByIdAsyc(winners[0].UserId);
                dUser.PlayedGames++;
                dUser.Wins++;
                dUser.Money += totalBet;
            }

            //lose
            foreach (var user in room.RoomUsers)
            {
                if (winners.Contains(user)) continue;

                //var sUser = user.Data;
                var dUser = await _masterRepo.GetUserByIdAsyc(user.UserId);
                dUser.PlayedGames++;
            }

            _masterRepo.SaveChanges();
        }

        private void NextTurn(Room room)
        {
            room.CurrentTurn = ++room.CurrentTurn % room.Capacity;
            StartTurn(room.RoomUsers[room.CurrentTurn]);
        }

        private async void StartTurn(RoomUser roomUser)
        {
            await _serverLoop.SetupTurnTimout(roomUser);
        }

        //rpc
        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        public async Task Ready(RoomUser roomUser)
        {
            roomUser.IsReady = true;
            await CheckAllPlayersAreReady(roomUser.Room);
        }

        private async Task CheckAllPlayersAreReady(Room room)
        {
            var readyUsersCount = room.RoomUsers.Count(u => u.IsReady);
            if (readyUsersCount == room.Capacity)
            {
                await InitialDistribute(room);
                StartTurn(room.RoomUsers[0]);
            }
        }

        private async Task InitialDistribute(Room room)
        {
            foreach (var roomUser in room.RoomUsers)
            {
                roomUser.Hand = roomUser.Room.Deck.CutRange(RoomUser.HandSize);
                await _masterHub.Clients.User(roomUser.UserId)
                    .SendAsync("InitialDistribute", roomUser.Hand.ToArray(), roomUser.Room.GroundCards.ToArray());
            }
        }

        private async Task Distribute(Room room)
        {
            foreach (var roomUser in room.RoomUsers)
            {
                roomUser.Hand = roomUser.Room.Deck.CutRange(RoomUser.HandSize);
                await _masterHub.Clients.User(roomUser.UserId).SendAsync("Distribute", roomUser.Hand.ToArray());
            }
        }

        public async Task RandomPlay(RoomUser roomUser)
        {
            var randomCardIndex = StaticRandom.GetRandom(roomUser.Hand.Count);

            await Task.WhenAll
            (
                Play(roomUser, randomCardIndex),
                _masterHub.Clients.User(roomUser.UserId)
                    .SendAsync("OverrideMyLastThrow", randomCardIndex)
            );
        }

        // rpc
        public async Task Play(RoomUser roomUser, int cardIndexInHand)
        {
            if (roomUser.TurnId != roomUser.Room.CurrentTurn || !cardIndexInHand.InRange(roomUser.Hand.Count))
                throw new BadUserInputException();
            //this is invoked by the server also, and may be a server error and it's handle way is ignoring and terminate the action
            //hub exc are not handled when the actor is the system

            _serverLoop.CutTurnTimout(roomUser);

            var eaten = RoomLogic.Eat(roomUser.Hand[cardIndexInHand], roomUser.Room.GroundCards, out bool basra,
                out bool bigBasra);
            roomUser.Room.GroundCards.RemoveAll(c => eaten.Contains(c));

            roomUser.EatenCardsCount += eaten.Count;
            if (basra) roomUser.BasraCount++;
            if (bigBasra) roomUser.BigBasraCount++;

            NextTurn(roomUser.Room);

            await _masterHub.Clients.GroupExcept("room" + roomUser.Room.Id, roomUser.ConnectionId)
                .SendAsync("CurrentOppoThrow", roomUser.Hand[cardIndexInHand]);
            //what do you mean by await?, waiting for deliver or timeout?

            if (roomUser.Hand.Count == 0 && roomUser.TurnId == roomUser.Room.Capacity - 1)
            {
                await Distribute(roomUser.Room);
            }
        }


        public async Task RequestFriendlyRoom(int[] userIds, int bet, int capacity)
        {
            throw new NotImplementedException();
        }

        public async Task BuyCardBack()
        {
            throw new NotImplementedException();
        }

        public async Task BuyBackground()
        {
            throw new NotImplementedException();
        }
    }
}