using System.Collections.Generic;

namespace Basra.Models.Client
{
    public class ActiveRoomState
    {
        public List<FullUserInfo> FullUsersInfo;
        public int TurnId;
        public List<int> MyHand;
        public List<int> OppoHandCounts;
        public List<int> Ground;
        public int CurrentTurn;
        public List<int> EatenCardsCounts;
        public List<int> BasrasCounts;
        public List<int> BigBasrasCounts;
    }
}