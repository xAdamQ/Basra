using System.Collections.Generic;

public class FullUserInfo : MinUserInfo
{
    public int PlayedRoomsCount { get; set; }
    public int WonRoomsCount { get; set; }
    public int EatenCardsCount { get; set; }
    public int WinStreak { get; set; }
    public int BasrasCount { get; set; }
    public int BigBasrasCount { get; set; }
    public int TotalEarnedMoney { get; set; }
    public List<int> OwnedCardBackIds { get; set; }
    public List<int> OwnedBackgrounds { get; set; }

    public float WinRatio => (float) WonRoomsCount / PlayedRoomsCount;
}