namespace Basra.Server
{
    public class RoomUser : RoomActor
    {
        public string ConnectionId { get; set; }

        public bool IsReady { get; set; }

        public ActiveUser ActiveUser { get; set; }
    }
}