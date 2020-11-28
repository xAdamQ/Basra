namespace Basra.Server.Structure
{
    public class User
    {
        public string Id;
        public string ConnectionId { get; set; }
        public bool Disconncted { get; set; }

        public Room.User RUser;
    }
}