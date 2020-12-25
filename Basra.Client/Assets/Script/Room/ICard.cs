namespace Basra.Client.Room
{
    public interface ICard
    {
        void AddFront(int id);
        void OppoThrow(int cardId);
        void Throw();
    }
}