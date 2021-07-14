using System.Collections.Generic;

namespace Basra.Models.Client
{
    public class ActiveRoomState
    {
        public int BetChoice { get; set; }
        public int CapacityChoice { get; set; }
        public int CurrentTurn { get; set; }

        public List<FullUserInfo> UserInfos { get; set; }
        public List<int> HandCounts { get; set; }
        //these guaranteed to have the same index

        public int MyTurnId { get; set; }
        public List<int> MyHand { get; set; }
        //my player data

        public List<int> Ground { get; set; }
    }
}