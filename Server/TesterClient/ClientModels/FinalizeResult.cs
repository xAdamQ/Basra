using System.Collections.Generic;

namespace Basra.Models.Client
{
    public class FinalizeResult
    {
        public RoomXpReport RoomXpReport { set; get; }
        public PersonalFullUserInfo PersonalFullUserInfo { set; get; }
        public int LastEaterTurnId { get; set; }
        public List<UserRoomStatus> UserRoomStatus { set; get; }
    }

    public class RoomXpReport
    {
        public int Competition { get; set; }
        public int Basra { get; set; }
        public int BigBasra { get; set; }
        public int GreatEat { get; set; }
    }

    public class UserRoomStatus
    {
        public int EatenCards { get; set; }
        public int Basras { get; set; }
        public int BigBasras { get; set; }
        public int WinMoney { get; set; }
    }

}