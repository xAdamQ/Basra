using System.Collections.Generic;
using Basra.Server.Extensions;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System;
using Basra.Server.Exceptions;
using System.Threading.Tasks;
using System.Timers;

namespace Basra.Server.Structure.Room
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
        public User[] Users { get; }
        public Ground Ground { get; }
        public bool[] IsReady { get; }
        public int Id { get; }
        private static int LastId { get; set; }
        public List<int> Deck { get; }
        public int CurrentTurn { get; private set; }
        private User UserInTurn => Users[CurrentTurn];

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
        }

        public Active(Pending pendingRoom)
        {
            All.Add(this);

            Genre = pendingRoom.Genre;
            Users = pendingRoom.Users.ToArray();
            Id = LastId++;

            IsReady = new bool[Users.Length];

            Deck = GenerateDeck();

            Ground = new Ground(Deck.CutRange(User.HandSize));

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

        private void InitialDistribute()
        {
            for (int u = 0; u < Users.Length; u++)
            {
                Users[u].InitialDistribute();
            }

        }

        private void Distribute()
        {
            //every player should have 4 cards or less
            //if the deck is 52 always, you won't need to customize hand size
            // if (Users.Length * HandSize > Deck.Count)
            // {
            //     var minHandSize = Deck.Count / Users.Length;
            //     var remainder = Deck.Count % Users.Length;

            //     int u = 0;
            //     for (; u < Users.Length; u++)
            //     {
            //         Distribute(u, minHandSize + remainder);
            //     }
            //     for (; u < Users.Length - remainder; u++)
            //     {
            //         Distribute(u, minHandSize);
            //     }
            // }
            // else
            // {

            for (int u = 0; u < Users.Length; u++)
            {
                Users[u].Distribute();
                //Distribute(u, HandSize);
            }
        }

        public void Ready(string playerId)
        {
            IsReady[GetUserRoomId(playerId)] = true;
            var readyUsersCount = IsReady.Count(value => value);
            if (readyUsersCount == Users.Length)
            {
                InitialDistribute();
            }
        }

        public void NextTurn()
        {
            CurrentTurn = ++CurrentTurn % Users.Length;
            UserInTurn.StartTurn();
        }

        #region helpers
        private int GetUserRoomId(string userId) => Array.FindIndex(Users, u => u.Structure.Id == userId);

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