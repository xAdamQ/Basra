using System.Collections.Generic;

namespace Basra.Client.Room
{
    public interface IUser
    {
        List<Card> Cards { get; set; }
        IRoomManager Room { get; set; }
        int TurnId { get; set; }
        TurnTimer TurnTimer { get; set; }
        UserType Type { get; set; }

        void CancelTurn();
        void CreateCards_Oppo();
        void CreateCards_Me(int[] hand);
        void StartTurn();
        void SetName(string name);
    }
}