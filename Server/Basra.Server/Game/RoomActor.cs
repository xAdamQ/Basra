using System.Collections.Generic;

namespace Basra.Server
{
    public abstract class RoomActor
    {
        public const int HandSize = 4;
        public const int HandTime = 11;

        public string Id { get; set; }

        public int BasraCount { get; set; }
        public int BigBasraCount { get; set; }
        public int EatenCardsCount { get; set; }

        public Room Room { get; set; }

        public List<int> Hand { get; set; }

        /// <summary>
        /// id in room, turn id
        /// </summary>
        public int TurnId;
    }
}