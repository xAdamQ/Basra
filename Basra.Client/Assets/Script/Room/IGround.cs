namespace Basra.Client.Room
{
    public interface IGround
    {
        void CreateInitialCards(int[] ground);
        void Distribute(Card card);
        Card MakeCard(int id);
        Card[] ThrowPt1(Card card);
        void ThrowPt2(Card card);
    }
}