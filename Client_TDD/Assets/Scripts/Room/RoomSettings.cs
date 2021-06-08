using System.Collections.Generic;

public class RoomSettings
{
    public RoomSettings(int betChoice, int capacityChoice, List<RoomOppoInfo> opposInfo, int myTurn)
    {
        BetChoice = betChoice;
        CapacityChoice = capacityChoice;
        OpposInfo = opposInfo;
        MyTurn = myTurn;
    }

    public int MyTurn { get; set; }

    public List<RoomOppoInfo> OpposInfo { get; set; }

    public int BetChoice { get; }
    public int Bet => Bets[BetChoice];
    private static readonly int[] Bets = {50, 250, 500, 1000};

    public int CapacityChoice { get; }
    public int Capacity => Capacities[CapacityChoice];
    private static readonly int[] Capacities = {2, 3, 4};
}