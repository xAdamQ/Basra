namespace Basra.Client.Room
{
    public interface IRoomManager
    {
        void CurrentOppoThrow(int cardId);
        void CurrentOverrideThrow(int cardIndex);
        void Distribute(int[] hand);
        void InitialDistribute(int[] hand, int[] ground);
        void NextTurn();
        void Ready();
        void RevertTurn();

        int CurrentTurn { get; set; }
        Ground Ground { get; set; }
    }
}