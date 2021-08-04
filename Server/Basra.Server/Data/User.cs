using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Basra.Server
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Fbid { get; set; }

        public int PlayedRoomsCount { get; set; }
        public int WonRoomsCount { get; set; }
        public int Draws { get; set; }

        public int Money { get; set; }

        public string Name { get; set; }

        public string PictureUrl { get; set; }

        //states
        public int EatenCardsCount { get; set; }
        public int WinStreak { get; set; }
        public int MaxWinStreak { get; set; }
        public int BasraCount { get; set; }
        public int BigBasraCount { get; set; }
        public int TotalEarnedMoney { get; set; }


        public int Level { get; set; }
        public int XP { get; set; }

        /// <summary>
        /// amount of requested aids today
        /// </summary>
        public int RequestedMoneyAidToday { get; set; }

        /// <summary>
        /// when it was requested, used to help the client see remaining time
        /// after reconnect
        /// </summary>
        public DateTime? LastMoneyAimRequestTime { get; set; }

        /// <summary>
        /// the player is waiting 15 minutes to get the money
        /// </summary>
        public bool IsMoneyAidProcessing => LastMoneyAimRequestTime != null;

        public List<UserRelation> Relations { get; set; }

        public List<int> OwnedBackgroundIds { get; set; } = new();
        public List<int> OwnedCardBackIds { get; set; } = new();
        public List<int> OwnedTitleIds { get; set; } = new();

        public int SelectedTitleId { get; set; }
        public int SelectedCardback { get; set; }
        public int SelectedBackground { get; set; }
    }

    public class UserRelation
    {
        public User User { get; set; }
        public string UserId { get; set; }

        public User OtherUser { get; set; }
        public string OtherUserId { get; set; }

        public int RelationType { get; set; }
    }
}