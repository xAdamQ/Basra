using System.Collections.Generic;
using Basra.Models.Client;

public class ThrowResult
{
    public int ThrownCard { set; get; }
    public List<int> EatenCardsIds { set; get; }
    public bool Basra { set; get; }
    public bool BigBasra { set; get; }
}

public class FinalizeResult
{
    public RoomXpReport RoomXpReport { set; get; }
    public PersonalFullUserInfo PersonalFullUserInfo { set; get; }
    public int LastEaterTurnId { get; set; }
}