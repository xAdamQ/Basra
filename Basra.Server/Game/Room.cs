using System.Collections.Generic;
using Basra.Server.Extensions;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System;
using Basra.Server.Exceptions;
using System.Threading.Tasks;
using System.Timers;

namespace Basra.Server
{
    public enum CardNames
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Boy,
        Girl,
        Old
    }

    public enum ShapeNames
    {
        Club,
        Diamond,
        Heart,
        Spade
    }

    // public interface IRoom
    // {
    //     int DeckSize { get; }
    //     int ShapeSize { get; }
    //     int Genre { get; }
    //     
    //     List<RoomUser> RoomUsers { get; }
    //     
    //     int UserCount { get; }
    //     List<int> GroundCards { get; }
    //     int Id { get; }
    //     List<int> Deck { get; }
    //     int CurrentTurn { get; }
    //     int EnteredUsersCount { get; set; }
    //
    //     Task AddUser(IHubContext<MasterHub> hub, IMasterRepo masterRepo, RoomUser rUser);
    //     Task CheckPlayersReady();
    //     Task Distribute();
    //     Task FinalizeGame(IMasterRepo masterRepo);
    //     void NextTurn();
    //     Task Start(IMasterRepo masterRepo);
    // }

    public class Room
    {
        //todo ready timeout
        //timeouts overall as defensive strategy
        //todo action that happened after it's time e.g. play card

        public const int DeckSize = 52;
        public int ShapeSize => 13;

        public List<int> GroundCards { get; set; }

        public int Genre { get; set; }

        public List<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();
        // public int UserCount { get; }

        // public List<int> GroundCards { get; set; }
        public int Id { get; set; }

        public List<int> Deck { get; set; }
        public int CurrentTurn { get; set; }
        public int Capacity { get; set; }

        // public int EnteredUsersCount { get; set; }
        // private RoomUser UserInTurn => null;// RoomUsers[CurrentTurn];
        //
        // private static int LastId { get; set; }
        public static readonly int[] GenreBets = new int[] {50, 100, 200};

        //
        private int TotalBet { get; set; }

        // public Room(int genre, int playerCount)
        // {
        //     Genre = genre;
        //     Capacity = playerCount;
        //
        //     // RoomUsers = new List<RoomUser>();
        //     // Id = LastId++;
        //
        //     TotalBet = GenreBets[Genre] * Capacity;
        //
        //     Deck = GenerateDeck();
        //
        //     GroundCards = Deck.CutRange(RoomUser.HandSize);
        // }

        // public async static Task AskForRoom(MasterHub hub, RoomFactory roomFactory, IMasterRepo masterRepo, int genre, int playerCount)
        // {
        //     IRoom pRoom = null;
        //     try //find a room with this specs
        //     {
        //         pRoom = All.First(r => r.Genre == genre && r.PlayerCount == playerCount);
        //     } //otherwise make new one
        //     catch (InvalidOperationException)
        //     {
        //         pRoom = roomFactory.MakeRoom(genre, playerCount);
        //         // pRoom = new Room(genre, playerCount);
        //         Console.WriteLine("a new room is made");
        //     }
        //
        //     var roomUser = new RoomUser(hub.Context.ConnectionId, hub.Context.UserIdentifier);
        //
        //     await pRoom.AddUser(hub, masterRepo, roomUser);
        // }

        // public async Task AddUser(IHubContext<MasterHub> hub, IMasterRepo masterRepo, RoomUser rUser)
        // {
        //     RoomUsers.RemoveAll(u => !masterRepo.GetUserActiveState(u.UserId));
        //     RoomUsers.Add(rUser);
        //
        //     if (UserCount == RoomUsers.Count)
        //     {
        //         Console.WriteLine("congrats, a room is ready");
        //
        //         await Start(masterRepo);
        //     }
        //     else
        //     {
        //         await hub.Clients.User(rUser.UserId).SendAsync("RoomIsFilling");
        //         // await hub.Clients.Caller.SendAsync("RoomIsFilling");
        //     }
        // }
        //
        // public async Task Start(IMasterRepo masterRepo)
        // {
        //     var userNames = new string[UserCount];
        //     for (int i = 0; i < UserCount; i++)
        //     {
        //         userNames[i] = await masterRepo.GetNameOfUserAsync(RoomUsers[i].UserId);
        //     }
        //     //var userNames = Users.Select(u => u.ActiveUser.Name).ToArray();
        //
        //     var tasks = new List<Task>();
        //     for (int i = 0; i < UserCount; i++)
        //     {
        //         tasks.Add(Program.HubContext.Groups.AddToGroupAsync(RoomUsers[i].ConnectionId, "room" + Id));
        //         tasks.Add(RoomUsers[i].StartRoom(this, i, userNames));
        //     }
        //
        //     await Task.WhenAll(tasks);
        //
        //     RoomUsers[0].StartTurn();
        // }
        //
        // private List<int> GenerateDeck()
        // {
        //     var deck = new List<int>();
        //     for (int i = 0; i < DeckSize; i++)
        //     {
        //         deck.Add(i);
        //     }
        //     deck.Shuffle();
        //     return deck;
        // }
        //
        // //factory is strongly coupled to the dependancy and the dependant
        // //factory makes an insterface implementation, but the returned type is interface
        // //factory is intended to hide creating instances logic
        // //factory is the solution for the multiple implmentations for one interface
        // //modern factories use the IServiceProvider to resolve the dependancies automatically (GetService() instead of new), still no config at runtime
        //
        // //anything needs injecting is a service
        //
        // //request from mw to hub to here
        //
        // private async Task InitialDistribute()
        // {
        //     var tasks = new Task[UserCount];
        //     for (int u = 0; u < UserCount; u++)
        //     {
        //         tasks[u] = RoomUsers[u].InitialDistribute();
        //     }
        //
        //     await Task.WhenAll(tasks);
        // }
        //
        // public async Task Distribute()
        // {
        //     var tasks = new Task[UserCount];
        //     for (int u = 0; u < UserCount; u++)
        //     {
        //         tasks[u] = RoomUsers[u].Distribute();
        //     }
        //
        //     await Task.WhenAll(tasks);
        // }
        //
        // public async Task CheckPlayersReady()
        // {
        //     //var readyUsersCount = IsReady.Count(value => value);
        //     var readyUsersCount = RoomUsers.Count(u => u.IsReady);
        //     if (readyUsersCount == UserCount)
        //     {
        //         await InitialDistribute();
        //     }
        // }
        // //the usage of ready is to make a valid call in the client, 
        // //we can get around this by storing the values in the client and use it when scene laods
        // //but this is not fair in case the client has slow device
        //
        // public void NextTurn()
        // {
        //     CurrentTurn = ++CurrentTurn % UserCount;
        //     UserInTurn.StartTurn();
        // }
        //
        // public async Task FinalizeGame(IMasterRepo masterRepo)
        // {
        //     masterRepo.DeleteRoom(this);
        //     
        //     var biggestEatenCount = RoomUsers.Max(u => u.EatenCardsCount);
        //     var biggestEaters = RoomUsers.Where(u => u.EatenCardsCount == biggestEatenCount);
        //
        //     for (int u = 0; u < UserCount; u++)
        //     {
        //         RoomUsers[u].Score =
        //             RoomUsers[u].BasraCount * 10 +
        //             RoomUsers[u].BigBasraCount * 30 +
        //             (biggestEaters.Contains(RoomUsers[u]) ? 30 : 0);
        //     }
        //
        //     var maxScore = RoomUsers.Max(u => u.Score);
        //     var winners = RoomUsers.Where(u => u.Score == maxScore).ToArray();
        //
        //     //draw
        //     if (winners.Length > 1)
        //     {
        //         var moneyPart = TotalBet / winners.Length;
        //         foreach (var user in winners)
        //         {
        //             //var sUser = user.ActiveUser.Data;
        //             var dUser = await masterRepo.GetUserByIdAsyc(user.UserId);
        //             dUser.PlayedGames++;
        //             dUser.Draws++;
        //             dUser.Money += moneyPart;
        //         }
        //     }
        //     //win
        //     else
        //     {
        //         //var sUser = winners[0].Data;
        //         var dUser = await masterRepo.GetUserByIdAsyc(winners[0].UserId);
        //         dUser.PlayedGames++;
        //         dUser.Wins++;
        //         dUser.Money += TotalBet;
        //     }
        //
        //     //lose
        //     foreach (var user in RoomUsers)
        //     {
        //         if (winners.Contains(user)) continue;
        //
        //         //var sUser = user.Data;
        //         var dUser = await masterRepo.GetUserByIdAsyc(user.UserId);
        //         dUser.PlayedGames++;
        //     }
        //
        //     masterRepo.SaveChanges();
        // }

        #region helpers

        //private int GetUserRoomId(string userId) => Array.FindIndex(Users, u => u.ActiveUser.Id == userId);

        /// <summary>
        /// check if the given index is equal to all similar indices, e.g. 7, 7+13, 7+26 (different colors of 7)
        // /// </summary>
        // private bool CheckNumberInAllShapes(CardNames cardName, int id)
        // {
        //     //cards = [][] color->num
        //     //fill them in deck
        //     //the identifier would be V2, or array
        //     var firstNumberId = (int)cardName;
        //     for (int i = 0; i < 4; i++)
        //     {
        //         if (firstNumberId + (ShapeSize * i) == id)
        //             return true;
        //     }
        //
        //     return false;
        // }

        #endregion
    }
}