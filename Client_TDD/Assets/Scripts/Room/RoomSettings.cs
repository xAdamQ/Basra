using System.Collections.Generic;
using Basra.Common;

public class RoomSettings
{
    public static RoomSettings I;

    private RoomSettings()
    {
        I = this;
    }

    public RoomSettings(int betChoice, int capacityChoice, List<FullUserInfo> userInfos, int myTurn) : this()
    {
        BetChoice = betChoice;
        CapacityChoice = capacityChoice;
        UserInfos = userInfos;
        MyTurn = myTurn;
    }

    public RoomSettings(ActiveRoomState activeRoomState) : this()
    {
        BetChoice = activeRoomState.BetChoice;
        CapacityChoice = activeRoomState.CapacityChoice;
        UserInfos = activeRoomState.UserInfos;
        MyTurn = activeRoomState.MyTurnId;
    }

    public int MyTurn { get; }
    public List<FullUserInfo> UserInfos { get; }
    public int BetChoice { get; }
    public int CapacityChoice { get; }


    public int Bet => Bets[BetChoice];
    public static int[] Bets => new[] {55, 110, 220};

    public int Capacity => Capacities[CapacityChoice];
    public static readonly int[] Capacities = {2, 3, 4};

    public static int MinBet => Bets[0];


    public int BetMoneyToPay()
    {
        return Bet;
    }
    public int BetWinShown()
    {
        return (int) ((Bet - (Bet * .1f)) * 2);
    } //not used in finalize panel
}