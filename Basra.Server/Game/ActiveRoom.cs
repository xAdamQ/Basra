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
    public enum CardNames { One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Boy, Girl, Old }
    public enum ShapeNames { Club, Diamond, Heart, Spade }


    //why?
    //when you want to acess from cached room user you don't have to pull from db
    //data exist in db and context
    //public class ActiveUser
    //{
    //    public string ConnectionId { get; set; }

    //}

    public class ActiveRoom
    {
        //todo ready timeout
        //timouts overall as defensive strategy
        //todo action that happened after it's time e.g. play card

        public int DeckSize => 52;
        public int ShapeSize => 13;

        public int Genre { get; }
        public IRoomUser[] Users { get; }
        public List<int> GroundCards { get; }
        public int Id { get; }
        public List<int> Deck { get; }
        public int CurrentTurn { get; private set; }
        private IRoomUser UserInTurn => Users[CurrentTurn];

        private static int LastId { get; set; }
        private static readonly int[] GenreBets = new int[] { 50, 100, 200 };

        public int TotalBet;

        public static List<ActiveRoom> All { get; } = new List<ActiveRoom>();

        public ActiveRoom(IPendingRoom pendingRoom)
        {
            All.Add(this);

            Genre = pendingRoom.Genre;
            Users = pendingRoom.Users.ToArray();
            Id = LastId++;

            TotalBet = GenreBets[Genre] * Users.Length;

            Deck = GenerateDeck();

            GroundCards = Deck.CutRange(RoomUser.HandSize);
        }

        public async Task Start(IMasterRepo masterRepo)
        {
            var userNames = new string[Users.Length];
            for (int i = 0; i < Users.Length; i++)
            {
                userNames[i] = await masterRepo.GetNameOfUserAsync(Users[i].UserId);
            }
            //var userNames = Users.Select(u => u.ActiveUser.Name).ToArray();

            var tasks = new List<Task>();
            for (int i = 0; i < Users.Length; i++)
            {
                tasks.Add(Program.HubContext.Groups.AddToGroupAsync(Users[i].ConnectionId, "room" + Id));
                tasks.Add(Users[i].StartRoom(this, i, userNames));
            }

            await Task.WhenAll(tasks);

            Users[0].StartTurn();
        }

        private List<int> GenerateDeck()
        {
            var deck = new List<int>();
            for (int i = 0; i < DeckSize; i++)
            {
                deck.Add(i);
            }
            deck.Shuffle();
            return deck;
        }

        //factory is strongly coupled to the dependancy and the dependant
        //factory makes an insterface implementation, but the returned type is interface
        //factory is intended to hide creating instances logic
        //factory is the solution for the multiple implmentations for one interface
        //modern factories use the IServiceProvider to resolve the dependancies automatically (GetService() instead of new), still no config at runtime

        //anything needs injecting is a service

        //request from mw to hub to here

        private async Task InitialDistribute()
        {
            var tasks = new Task[Users.Length];
            for (int u = 0; u < Users.Length; u++)
            {
                tasks[u] = Users[u].InitialDistribute();
            }

            await Task.WhenAll(tasks);
        }

        public async Task Distribute()
        {
            var tasks = new Task[Users.Length];
            for (int u = 0; u < Users.Length; u++)
            {
                tasks[u] = Users[u].Distribute();
            }

            await Task.WhenAll(tasks);
        }

        public async Task CheckPlayersReady()
        {
            //var readyUsersCount = IsReady.Count(value => value);
            var readyUsersCount = Users.Count(u => u.IsReady);
            if (readyUsersCount == Users.Length)
            {
                await InitialDistribute();
            }
        }
        //the usage of ready is to make a valid call in the client, 
        //we can get around this by storing the values in the client and use it when scene laods
        //but this is not fair in case the client has slow device

        public void NextTurn()
        {
            CurrentTurn = ++CurrentTurn % Users.Length;
            UserInTurn.StartTurn();
        }

        public async Task FinalizeGame(IMasterRepo masterRepo)
        {
            var biggestEatenCount = Users.Max(u => u.EatenCardsCount);
            var biggestEaters = Users.Where(u => u.EatenCardsCount == biggestEatenCount);

            for (int u = 0; u < Users.Length; u++)
            {
                Users[u].Score =
                    Users[u].BasraCount * 10 +
                    Users[u].BigBasraCount * 30 +
                    (biggestEaters.Contains(Users[u]) ? 30 : 0);
            }

            var maxScore = Users.Max(u => u.Score);
            var winners = Users.Where(u => u.Score == maxScore).ToArray();

            //draw
            if (winners.Length > 1)
            {
                var moneyPart = TotalBet / winners.Length;
                foreach (var user in winners)
                {
                    //var sUser = user.ActiveUser.Data;
                    var dUser = await masterRepo.GetUserByIdAsyc(user.UserId);
                    dUser.PlayedGames++;
                    dUser.Draws++;
                    dUser.Money += moneyPart;
                }
            }
            //win
            else
            {
                //var sUser = winners[0].Data;
                var dUser = await masterRepo.GetUserByIdAsyc(winners[0].UserId);
                dUser.PlayedGames++;
                dUser.Wins++;
                dUser.Money += TotalBet;
            }

            //lose
            foreach (var user in Users)
            {
                if (winners.Contains(user)) continue;

                //var sUser = user.Data;
                var dUser = await masterRepo.GetUserByIdAsyc(user.UserId);
                dUser.PlayedGames++;
            }

            masterRepo.SaveChanges();
        }

        #region helpers
        //private int GetUserRoomId(string userId) => Array.FindIndex(Users, u => u.ActiveUser.Id == userId);

        /// <summary>
        /// check if the given index is equal to all similar indices, e.g. 7, 7+13, 7+26 (different colors of 7)
        /// </summary>
        private bool CheckNumberInAllShapes(CardNames cardName, int id)
        {
            //cards = [][] color->num
            //fill them in deck
            //the identifier would be V2, or array
            var firstNumberId = (int)cardName;
            for (int i = 0; i < 4; i++)
            {
                if (firstNumberId + (ShapeSize * i) == id)
                    return true;
            }

            return false;
        }
        #endregion

    }
}