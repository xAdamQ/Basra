using System.Collections.Generic;
using Basra.Models.Client;

public class ThrowResult
{
    public int ThrownCard;
    public List<int> EatenCardsIds;
    public bool Basra;
    public bool BigBasra;

    public FinalizeResult FinalizeResult;
    public DistributeResult DistributeResult;
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

//currentoppothrow, forceplay