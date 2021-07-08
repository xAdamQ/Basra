using System.Collections.Generic;

public class ActiveRoomState
{
    public int BetChoice { get; set; }
    public int CapacityChoice { get; set; }
    public List<FullUserInfo> UserInfos { get; set; }
    public int MyTurnId { get; set; }
    //room static data

    public int CurrentTurn { get; set; }
    public List<int> HandCounts { get; set; }
    public List<int> MyHand { get; set; }
    public List<int> Ground { get; set; }
    //state specific data
}