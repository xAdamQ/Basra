using Basra.Server.Extensions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Basra.Server.Structure.Room
{
    public class User
    {
        public Structure.User Structure;
        public static int HandSize => 4;

        //props for active, so if the user made ready and cancel, this will die without usage
        public Active Active { get; set; }
        public List<int> Hand { get; set; }
        public int HandTime { get; private set; }

        /// <summary>
        /// id in room
        /// </summary>
        public int Id;

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

        public bool IsMyTurn()
        {
            return Id == Active.CurrentTurn;
        }

        public async void OnTurnTimeout(object state)
        {
            await RandomPlay();
        }

        //rpc
        public async Task<int[]> Play(int cardIndexInHand)
        {
            if (!IsMyTurn())
                throw new Exception();

            var card = Hand.Cut(cardIndexInHand);

            Active.NextTurn();

            await Active.ResetTurnTimer();

            return Active.Ground.Eat(card);//sibling relation is not premited
        }
        //rev rpc
        public async Task<int[]> RandomPlay()
        {
            var randomCardIndex = StaticRandom.GetRandom(Hand.Count);
            return await Play(randomCardIndex);
        }

    }
}
