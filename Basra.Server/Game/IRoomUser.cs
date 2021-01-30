//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Basra.Server
//{
//    public interface IRoomUser
//    {
//        //ActiveUser ActiveUser { get; set; }

//        ActiveRoom ActiveRoom { get; set; }
//        int BasraCount { get; set; }
//        int BigBasraCount { get; set; }
//        List<int> Cards { get; set; }
//        int Score { get; set; }
//        int EatenCardsCount { get; set; }

//        string ConnectionId { get; set; }
//        string UserId { get; set; }

//        bool IsReady { get; set; }

//        Task Distribute();
//        Task InitialDistribute();
//        bool IsMyTurn();
//        Task Play(int cardIndexInHand);
//        Task RandomPlay();
//        Task Ready();
//        Task StartRoom(ActiveRoom active, int id, string[] playerNames);
//        void StartTurn();
//    }
//}