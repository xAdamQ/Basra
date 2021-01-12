using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basra.Server.Room
{
    public interface IUser
    {
        Data.IUser Structure { get; set; }

        Active Active { get; set; }
        int BasraCount { get; set; }
        int BigBasraCount { get; set; }
        List<int> Cards { get; set; }
        int Score { get; set; }
        int EatenCardsCount { get; set; }

        Task Distribute();
        Task InitialDistribute();
        bool IsMyTurn();
        Task Play(int cardIndexInHand);
        Task RandomPlay();
        Task Ready();
        Task StartRoom(Active active, int id, string[] playerNames);
        void StartTurn();
    }
}