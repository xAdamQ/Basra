using System.Collections.Generic;
using Basra.Server.Extensions;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Basra.Server.Structure
{
    public class Room
    {
        //todo ready timeout
        //timouts overall as defensive strategy

        //wrong approach but I don't know the right
        public IHubContext<MasterHub> _hubContext { get; }

        public int DeckSize => 52;
        public int HandSize => 4;
        public int HandTime = 10;//7 in the client

        public int Genre { get; }
        public User[] Users { get; }
        public List<int>[] Hands { get; }
        public bool[] IsReady { get; }

        public int Id { get; }
        private static int LastId { get; set; }
        public List<int> Deck { get; }

        public static List<Room> All { get; } = new List<Room>();

        private int GetUserRoomId(string userId)
        {
            return Array.FindIndex(Users, u => u.Id == userId);
        }

        public Room(IHubContext<MasterHub> hubContext, PendingRoom pendingRoom)
        {
            _hubContext = hubContext;

            Genre = pendingRoom.Genre;
            Users = pendingRoom.Users.ToArray();
            Id = LastId++;

            foreach (var user in Users)
            {
                user.Room = this;
            }

            Hands = new List<int>[Users.Length];
            for (int i = 0; i < Hands.Length; i++)
            {
                Hands[i] = new List<int>();
            }

            IsReady = new bool[Users.Length];

            Deck = GenerateDeck();
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

        public void Distribute()
        {
            //every player should have 4 cards or less
            if (Users.Length * HandSize > Deck.Count)
            {
                var minHandSize = Deck.Count / Users.Length;
                var remainder = Deck.Count % Users.Length;

                int u = 0;
                for (; u < Users.Length; u++)
                {
                    SetHand(u, minHandSize + remainder);
                }
                for (; u < Users.Length - remainder; u++)
                {
                    SetHand(u, minHandSize);
                }
            }
            else
            {
                for (int u = 0; u < Users.Length; u++)
                {
                    SetHand(u, HandSize);
                }
            }
        }

        private void SetHand(int userRoomId, int handSize)
        {
            Hands[userRoomId] = Deck.CutRange(handSize);
            _hubContext.Clients.Client(Users[userRoomId].ConnectionId).SendAsync("SetHand", Hands[userRoomId]);
        }

        public void Ready(string playerId)
        {
            IsReady[GetUserRoomId(playerId)] = true;
            var readyUsersCount = IsReady.Count(value => value);
            if (readyUsersCount == Users.Length)
            {
                Distribute();
            }
        }
    }
}