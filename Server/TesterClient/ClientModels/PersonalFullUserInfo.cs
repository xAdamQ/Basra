using System;
using System.Collections.Generic;

namespace Basra.Models.Client
{
    public class PersonalFullUserInfo : FullUserInfo
    {
        public TimeSpan? MoneyAimTimeLeft { get; set; }

        public int FlipWinCount { get; set; }

        public List<int> Titles { get; set; }
    }
}