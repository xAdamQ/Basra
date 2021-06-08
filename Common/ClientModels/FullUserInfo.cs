using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Basra.Server.Models.Client
{
    public class FullUserInfo : MinUserInfo
    {
        public int PlayedRoomsCount { get; set; }
        public int WonRoomsCount { get; set; }
        public int EatenCardsCount { get; set; }
        public int WinStreak { get; set; }
        public int BasraCount { get; set; }
        public int BigBasraCount { get; set; }
        public int TotalEarnedMoney { get; set; }

        public List<int> OwnedCardBackIds { get; set; }
        public List<int> OwnedBackgroundsIds { get; set; }
    }
}