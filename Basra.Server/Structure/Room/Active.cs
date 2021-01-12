using System.Collections.Generic;
using Basra.Server.Extensions;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System;
using Basra.Server.Exceptions;
using System.Threading.Tasks;
using System.Timers;

namespace Basra.Server.Room
{
    public enum CardNames { One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Boy, Girl, Old }
    public enum ShapeNames { Club, Diamond, Heart, Spade }

    public class Active
    {

        //todo ready timeout
        //timouts overall as defensive strategy
        //todo action that happened after it's time e.g. play card

        public int DeckSize => 52;
        public int ShapeSize => 13;

        public int Genre { get; }
        public IUser[] Users { get; }
        public List<int> GroundCards { get; }
        public bool[] IsReady { get; }
        public int Id { get; }
        private static int LastId { get; set; }
        public List<int> Deck { get; }
        public int CurrentTurn { get; private set; }
        private IUser UserInTurn => Users[CurrentTurn];

        private static readonly int[] GenreBets = new int[] { 50, 100, 200 };

        public int TotalBet;

        public static List<Active> All { get; } = new List<Active>();

        public async Task Start()
        {
            var userNames = Users.Select(u => u.Structure.Name).ToArray();
            var tasks = new List<Task>();

            for (int i = 0; i < Users.Length; i++)
            {
                tasks.Add(Program.HubContext.Groups.AddToGroupAsync(Users[i].Structure.ConnectionId, "room" + Id));
                tasks.Add(Users[i].StartRoom(this, i, userNames));
            }

            await Task.WhenAll(tasks);

            Users[0].StartTurn();
        }

        public Active(IPending pendingRoom)
        {
            All.Add(this);

            Genre = pendingRoom.Genre;
            Users = pendingRoom.Users.ToArray();
            Id = LastId++;

            IsReady = new bool[Users.Length];

            TotalBet = GenreBets[Genre] * Users.Length;

            Deck = GenerateDeck();

            GroundCards = Deck.CutRange(User.HandSize);
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

        public async Task Ready(string playerId)
        {
            IsReady[GetUserRoomId(playerId)] = true;
            var readyUsersCount = IsReady.Count(value => value);
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

        public void FinalizeGame()
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
                    var sUser = user.Structure;
                    sUser.PlayedGames++;
                    sUser.Draws++;
                    sUser.Money += moneyPart;
                }
            }
            //win
            else
            {
                var sUser = winners[0].Structure;
                sUser.PlayedGames++;
                sUser.Wins++;
                sUser.Money += TotalBet;
            }

            //lose
            foreach (var user in Users)
            {
                if (winners.Contains(user)) continue;

                var sUser = user.Structure;
                sUser.PlayedGames++;
            }

        }

        #region helpers
        private int GetUserRoomId(string userId) => Array.FindIndex(Users, u => u.Structure.IdentityUserId == userId);

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