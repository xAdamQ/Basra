using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Basra.Common
{
    // [Preserve]
    public class ActiveRoomState
    {
        [Preserve]
        public ActiveRoomState() { }

        public int BetChoice { get; set; }
        public int CapacityChoice { get; set; }
        public List<FullUserInfo> UserInfos { get; set; }
        public int MyTurnId { get; set; }
        //room static data

        public int CurrentTurn { get; set; }
        public List<int> HandCounts { get; set; }
        public List<int> MyHand { get; set; }
        public List<int> Ground { get; set; }
        public bool LastHand { get; set; }
        //state specific data
    }
}