using Basra.Server.Extensions;
using Basra.Server.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.Linq;

namespace Basra.Server
{
    public class RoomUser
    {
        public static int HandSize => 4;
        public const int HandTime = 11;

        public string Id { get; set; }

        //props for active, so if the user made ready and cancel, this will die without usage
        public Room ActiveRoom { get; set; }

        // public List<int> Cards { get; set; }
        public int Score { get; set; }
        public int BasraCount { get; set; }
        public int BigBasraCount { get; set; }
        public int EatenCardsCount { get; set; }

        public bool IsReady { get; set; }

        /// <summary>
        /// we keep the room user even if he is disconnected
        /// </summary>
        public bool IsActive { get; set; }

        public string ConnectionId { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public Room Room { get; set; }

        public List<int> Hand { get; set; }

        /// <summary>
        /// id in room, turn id
        /// </summary>
        public int TurnId;

        //public ActiveUser ActiveUser { get; set; }

        // public RoomUser(string connId, string userId)
        // {
        //     UserId = userId;
        //     ConnectionId = connId;
        // }

        // public async Task StartRoom(Room room, int id, string[] playerNames)
        // {
        //     IdInRoom = id;
        //     ActiveRoom = room;
        //
        //     await Program.HubContext.Clients.User(UserId).SendAsync("StartRoom", IdInRoom, playerNames);
        // }

        //rpc
        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        // public async Task Ready()
        // {
        //     IsReady = true;
        //     await ActiveRoom.CheckPlayersReady();
        // }
        //
        // //rev rpc
        // public async Task InitialDistribute()
        // {
        //     Cards = ActiveRoom.Deck.CutRange(HandSize);
        //     await Program.HubContext.Clients.User(UserId)
        //         .SendAsync("InitialDistribute", Cards.ToArray(), ActiveRoom.GroundCards.ToArray());
        // }
        //
        // //rev rpc
        // public async Task Distribute()
        // {
        //     Cards = ActiveRoom.Deck.CutRange(HandSize);
        //     await Program.HubContext.Clients.User(UserId).SendAsync("Distribute", Cards.ToArray());
        // }
        //
        // CancellationTokenSource TurnTimoutCancelation;

        // public void StartTurn()
        // {
        //     TurnTimoutCancelation = new CancellationTokenSource();
        //     Task.Delay(HandTime * 1000).ContinueWith(t => RandomPlay(), TurnTimoutCancelation.Token);
        // }

        //rpc
        // public async Task Play(int cardIndexInHand)
        // {
        //     if (!IsMyTurn() || !cardIndexInHand.InRange(Cards.Count))
        //         throw
        //             new BadUserInputException(); //this is invoked by the server also, and may be a server error and it's handle way is ignoring and terminate the action
        //     //hub exc are not handled when the actor is the system
        //
        //     TurnTimoutCancelation.Cancel();
        //
        //     var eaten = RoomLogic.Eat(Cards[cardIndexInHand], ActiveRoom.GroundCards, out bool basra,
        //         out bool bigBasra);
        //     ActiveRoom.GroundCards.RemoveAll(c => eaten.Contains(c));
        //
        //     EatenCardsCount += eaten.Count;
        //     if (basra) BasraCount++;
        //     if (bigBasra) BigBasraCount++;
        //
        //     ActiveRoom.NextTurn();
        //
        //     await Program.HubContext.Clients.GroupExcept("room" + ActiveRoom.Id, ConnectionId)
        //         .SendAsync("CurrentOppoThrow", Cards[cardIndexInHand]);
        //     //what do you mean by await?, waiting for deliver or timeout?
        //
        //     if (Cards.Count == 0 && IdInRoom == ActiveRoom.UserCount - 1)
        //     {
        //         await ActiveRoom.Distribute();
        //     }
        // }

        //rev rpc
        // public async Task RandomPlay()
        // {
        //     var randomCardIndex = StaticRandom.GetRandom(Cards.Count);
        //
        //     await Task.WhenAll
        //     (
        //         Play(randomCardIndex),
        //         Program.HubContext.Clients.Client(ConnectionId).SendAsync("OverrideMyLastThrow", randomCardIndex)
        //         // Structure.SendAsync("OverrideThrow", card)
        //     );
        // }

        //public void ThrowToGround(int cardIndexInHand)
        //{
        //    var basra = false;
        //    var bigBasra = false;
        //    var cardId = Cards.Cut(cardIndexInHand);

        //    var cardValue = CardValueFromId(cardId);

        //    IEnumerable<int> eaten = null;

        //    if (cardId == KOMI_ID)
        //    {
        //        basra = Active.GroundCards.Count == 1;

        //        eaten = Active.GroundCards;
        //        Active.GroundCards.Clear();
        //    }
        //    else if (cardValue == BOY_VALUE)
        //    {
        //        bigBasra = Active.GroundCards.Count == 1 && BOY_IDS.Contains(Active.GroundCards[0]);

        //        eaten = Active.GroundCards;
        //        Active.GroundCards.Clear();
        //    }
        //    else if (cardValue > 10)
        //    {
        //        eaten = Active.GroundCards.Where(c => CardValueFromId(c) == cardValue);
        //        Active.GroundCards.RemoveAll(c => eaten.Contains(c));
        //    }
        //    else
        //    {
        //        var groups = Active.GroundCards.Select(c => CardValueFromId(c)).Permutations();

        //        var bestGroupLength = -1;
        //        int[] bestGroup = null;
        //        foreach (var group in groups)
        //        {
        //            if (group.Sum() == cardValue && group.Length > bestGroupLength)
        //            {
        //                bestGroup = group;
        //                bestGroupLength = bestGroup.Length;
        //            }
        //        }

        //        eaten = bestGroup;
        //        Active.GroundCards.RemoveAll(c => eaten.Contains(c));
        //    }
        //}

        // #region helpers

        // public bool IsMyTurn => TurnId == Room.CurrentTurn;

        // #endregion
    }

    public static partial class RoomLogic
    {
        private const int KOMI_ID = 19,
            BOY_VALUE = 11;

        private static readonly int[] BOY_IDS = new int[] {10, 23, 36, 49};

        public static List<int> Eat(int cardId, List<int> ground, out bool basra, out bool bigBasra)
        {
            basra = false;
            bigBasra = false;

            var cardValue = CardValueFromId(cardId);

            if (cardId == KOMI_ID)
            {
                basra = ground.Count == 1;

                return new List<int>(ground);
            }
            else if (cardValue == BOY_VALUE)
            {
                bigBasra = ground.Count == 1 && BOY_IDS.Contains(ground[0]);

                return new List<int>(ground);
            }
            else if (cardValue > 10)
            {
                return ground.Where(c => CardValueFromId(c) == cardValue).ToList();
            }
            else
            {
                var groups = ground.Permutations();

                var bestGroupLength = -1;
                int[] bestGroup = null;
                foreach (var group in groups)
                {
                    if (group.Select(c => CardValueFromId(c)).Sum() == cardValue && group.Length > bestGroupLength)
                    {
                        bestGroup = group;
                        bestGroupLength = bestGroup.Length;
                    }
                }

                basra = bestGroup.Length == ground.Count;

                return new List<int>(bestGroup);
            }

            int CardValueFromId(int id)
            {
                return (id % 13) + 1;
            }
        }
    }
}