using System;
using System.Collections.Generic;

namespace Basra.Server
{
    /// <summary>
    /// methods are service independent
    /// </summary>
    public class Room
    {
        //todo ready timeout, timeouts overall as defensive strategy
        //todo action that happened after it's time e.g. play card

        public const int DeckSize = 52;
        public const int ShapeSize = 13;

        //each bet has specific xp gain, when you lose for example you take only .25 only of it 
        public const float LoseXpPercent = .25f;
        public const float DrawXpPercent = .5f;
        public const float WinXpPercent = 1f;
        public const float BasraXpPercent = .05f;
        public const float BigBasraXpPercent = 1f;
        public const float GreatEatXpPercent = .3f;
        public const int GreatEatThreshold = 38;


        private const int MaxLevel = 999;
        public static int GetLevelFromXp(int xp)
        {
            var level = (int)(MathF.Pow(xp, .55f) / 10);
            return level < MaxLevel ? level : MaxLevel;
        }

        public List<int> GroundCards { get; set; }

        //don't bother yourself because it's readonly because if converted to database
        //the list will be handled differently
        public List<RoomUser> RoomUsers { get; } = new();
        public List<RoomActor> RoomActors { get; } = new();
        public List<RoomBot> RoomBots { set; get; } //left null on purpose

        public int Id { get; set; }

        public int BetChoice { get; }
        public int Bet => Bets[BetChoice];
        public static int[] Bets => new[] { 55, 110, 220 };

        /// <summary>
        /// used in money aim code
        /// </summary>
        public static int MinBet => Bets[0];

        public int CapacityChoice { get; }
        public int Capacity => Capacities[CapacityChoice];
        public static int[] Capacities => new[] { 2, 3, 4 };

        public List<int> Deck { get; set; }
        public int CurrentTurn { get; set; }

        public Room(int betChoice, int capacityChoice)
        {
            BetChoice = betChoice;
            CapacityChoice = capacityChoice;
        }

        public bool IsFull => RoomUsers.Count == Capacity;

        public void SetUsersDomains(Type domain)
        {
            foreach (var ru in RoomUsers) ru.ActiveUser.Domain = domain;
        }
    }
}