using Basra.Server.Extensions;
using Basra.Server.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Basra.Server.Data
{
    public class RoomUser
    {
        public const int HandSize = 4;
        public const int HandTime = 11;

        //props for active, so if the user made ready and cancel, this will die without usage
        //public ActiveRoom ActiveRoom { get; set; }
        public List<int> Cards { get; set; }

        public int Score { get; set; }
        public int BasraCount { get; set; }
        public int BigBasraCount { get; set; }
        public int EatenCardsCount { get; set; }

        public bool IsReady { get; set; }

        public string ConnectionId { get; set; }

        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        [ForeignKey("Room")]
        public string RoomId { get; set; }

        /// <summary>
        /// id in room, turn id
        /// </summary>
        public int TurnId;

        /// <summary>
        /// when active is made
        /// </summary>
        public async Task StartRoom(Room active, int id, string[] playerNames)
        {
            TurnId = id;
            ActiveRoom = active;

            await Program.HubContext.Clients.Client(ConnectionId).SendAsync("StartRoom", TurnId, playerNames);
        }

        //rpc
        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        public async Task Ready()
        {
            IsReady = true;
            await Room.CheckPlayersReady();
        }

        //rev rpc
        public async Task InitialDistribute()
        {
            Cards = Room.Deck.CutRange(HandSize);
            await Program.HubContext.Clients.Client(ConnectionId).SendAsync("InitialDistribute", Cards.ToArray(), Room.GroundCards.ToArray());
        }
        //rev rpc
        public async Task Distribute()
        {
            Cards = Room.Deck.CutRange(HandSize);
            await Program.HubContext.Clients.Client(ConnectionId).SendAsync("Distribute", Cards.ToArray());
        }

        CancellationTokenSource TurnTimoutCancelation;

        public void StartTurn()
        {
            TurnTimoutCancelation = new CancellationTokenSource();
            Task.Delay(HandTime * 1000).ContinueWith(t => RandomPlay(), TurnTimoutCancelation.Token);
        }

        //rpc
        public async Task Play(int cardIndexInHand)
        {
            if (!IsMyTurn() || !cardIndexInHand.InRange(Cards.Count))
                throw new BadUserInputException();//this is invoked by the server also, and may be a server error and it's handle way is ignoring and terminate the action
                                                  //hub exc are not handled when the actor is the system

            TurnTimoutCancelation.Cancel();

            var eaten = RoomLogic.Eat(Cards[cardIndexInHand], Room.GroundCards, out bool basra, out bool bigBasra);
            Room.GroundCards.RemoveAll(c => eaten.Contains(c));

            EatenCardsCount += eaten.Count;
            if (basra) BasraCount++;
            if (bigBasra) BigBasraCount++;

            Room.NextTurn();

            await Program.HubContext.Clients.GroupExcept("room" + Room.Id, ConnectionId).SendAsync("CurrentOppoThrow", Cards[cardIndexInHand]);
            //what do you mean by await?, waiting for deliver or timeout?

            if (Cards.Count == 0 && TurnId == Room.Users.Length - 1)
            {
                await Room.Distribute();
            }
        }
        //rev rpc
        public async Task RandomPlay()
        {
            var randomCardIndex = StaticRandom.GetRandom(Cards.Count);

            await Task.WhenAll
            (
                Play(randomCardIndex),
                Program.HubContext.Clients.Client(ConnectionId).SendAsync("OverrideMyLastThrow", randomCardIndex)
            // Structure.SendAsync("OverrideThrow", card)
            );
        }

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

        #region helpers

        public bool IsMyTurn() => TurnId == Room.CurrentTurn;

        #endregion
    }
}