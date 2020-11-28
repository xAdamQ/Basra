using Basra.Server.Extensions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;

namespace Basra.Server.Structure.Room
{
    public class User
    {
        public Structure.User Structure;
        public static int HandSize => 4;

        //props for active, so if the user made ready and cancel, this will die without usage
        public Active Active { get; set; }
        public List<int> Hand { get; set; }

        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        public void Ready()
        {
            Active.Ready(Structure.Id);
        }

        public void InitialDistribute()
        {
            Hand = Active.Deck.CutRange(HandSize);
            Program.HubContext.Clients.Client(Structure.ConnectionId).SendAsync("InitialDistribute", Hand.ToArray(), Active.Ground.Cards.ToArray());
        }
        public void Distribute()
        {
            Hand = Active.Deck.CutRange(HandSize);
            Program.HubContext.Clients.Client(Structure.ConnectionId).SendAsync("Distribute", Hand.ToArray());
        }

        public int[] Throw(int cardIndexInHand)
        {
            try
            {
                Active.Throw(this);
            }
            catch (Exception)
            {
                return null;
            }

            var card = Hand.Cut(cardIndexInHand);
            return Active.Ground.Throw(card);
        }

    }
}
