namespace Basra.Models.Client
{
    public class FinalizeResult
    {
        public RoomXpReport RoomXpReport { set; get; }
        public PersonalFullUserInfo PersonalFullUserInfo { set; get; }
        public int LastEaterTurnId { get; set; }
    }
}