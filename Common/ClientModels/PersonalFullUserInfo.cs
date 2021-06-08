using System;
using System.Collections.Generic;

namespace Basra.Server.Models.Client
{
    public class PersonalFullUserInfo : FullUserInfo
    {
        public int Money { get; set; }
        public TimeSpan? MoneyAimTimeLeft { get; set; }
        public int FlipWinCount { get; set; }
        public object ActiveRoomData { get; set; }
        public List<string> Titles { get; set; }
        public int SelectedCardback { get; set; }
        public int SelectedBackground { get; set; }
    }
}