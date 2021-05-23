public interface IRoomRepo
{
    FullUserInfo[] OpposInfo { get; set; }
    int BetChoice { get; }
    int Bet { get; }
    int CapacityChoice { get; }
    int Capacity { get; }
}

public class RoomRepo : IRoomRepo
{
    public struct Settings
    {
        public int BetChoice { get; }
        public int CapacityChoice { get; }
        public FullUserInfo[] OpposInfo { get; }

        public Settings(int betChoice, int capacityChoice, FullUserInfo[] opposInfo)
        {
            BetChoice = betChoice;
            CapacityChoice = capacityChoice;
            OpposInfo = opposInfo;
        }
    }

    public RoomRepo(Settings settings)
    {
        BetChoice = settings.BetChoice;
        CapacityChoice = settings.CapacityChoice;
        OpposInfo = settings.OpposInfo;
    }

    public FullUserInfo[] OpposInfo { get; set; }

    public int BetChoice { get; }
    public int Bet => Bets[BetChoice];
    private static readonly int[] Bets = {50, 250, 500, 1000};

    public int CapacityChoice { get; }
    public int Capacity => Capacities[CapacityChoice];
    private static readonly int[] Capacities = {2, 3, 4};
}