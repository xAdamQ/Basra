using Basra.Server.Extensions;
using Basra.Server.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace Basra.Server.Structure.Room
{
    public class User
    {
        public Structure.User Structure;
        public static int HandSize => 4;

        //props for active, so if the user made ready and cancel, this will die without usage
        public Active Active { get; set; }
        public List<int> Hand { get; set; }
        public const int HandTime = 11;

        // private Timer TurnTimer;

        /// <summary>
        /// id in room, turn id
        /// </summary>
        public int Id;

        /// <summary>
        /// when active is made
        /// </summary>
        public async Task StartRoom(Active active, int id, string[] playerNames)
        {
            Id = id;
            Active = active;

            await Program.HubContext.Clients.Client(Structure.ConnectionId).SendAsync("StartRoom", Id, playerNames);
        }

        //rpc
        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        public void Ready()
        {
            Active.Ready(Structure.Id);
        }

        //rev rpc
        public void InitialDistribute()
        {
            Hand = Active.Deck.CutRange(HandSize);
            Program.HubContext.Clients.Client(Structure.ConnectionId).SendAsync("InitialDistribute", Hand.ToArray(), Active.Ground.Cards.ToArray());
        }
        //rev rpc
        public void Distribute()
        {
            Hand = Active.Deck.CutRange(HandSize);
            Program.HubContext.Clients.Client(Structure.ConnectionId).SendAsync("Distribute", Hand.ToArray());
        }

        CancellationTokenSource TurnTimoutCancelation;
        public void StartTurn()
        {
            TurnTimoutCancelation = new CancellationTokenSource();
            Task.Delay(HandTime).ContinueWith(t => RandomPlay(), TurnTimoutCancelation.Token);
        }

        //rpc
        public async Task Play(int cardIndexInHand)
        {
            if (!IsMyTurn())
                throw new BadUserInputException();//this is invoked by the server also, and may be a server error and it's handle way is ignoring and terminate the action
            //hub exc are not handled when the actor is the system

            TurnTimoutCancelation.Cancel();

            var card = Hand.Cut(cardIndexInHand);

            Active.Ground.Eat(card);//sibling relation is not premited

            Active.NextTurn();

            await Program.HubContext.Clients.GroupExcept("room" + Active.Id, Structure.ConnectionId).SendAsync("OppoThrow", card);
        }
        //rev rpc
        public async Task RandomPlay()
        {
            var randomCardIndex = StaticRandom.GetRandom(Hand.Count);

            await Task.WhenAll
            (
                Play(randomCardIndex),
                Program.HubContext.Clients.Client(Structure.ConnectionId).SendAsync("OverrideThrow", randomCardIndex)
            // Structure.SendAsync("OverrideThrow", card)
            );
        }

        #region helpers

        public bool IsMyTurn()
        {
            return Id == Active.CurrentTurn;
        }


        #endregion
    }
}
