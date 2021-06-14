using System.Collections.Generic;

namespace Basra.Models.Client
{
    public class ThrowResult
    {
        public int ThrownCard;
        public List<int> EatenCardsIds;
        public bool Basra;
        public bool BigBasra;
    }

    public class FinalizeResult
    {
        public RoomXpReport RoomXpReport;
        public PersonalFullUserInfo PersonalFullUserInfo;
    }

    public class DistributeResult
    {
        public List<int> MyHand;
    }
}