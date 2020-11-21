namespace Basra.Server.Structure
{
    public class User
    {
        public string Id;
        public Room Room { get; set; }
        public PendingRoom PendingRoom { get; set; }
        public string ConnectionId { get; set; }
        public bool Disconncted { get; set; }

        public void PlayCard(int cardIndex)
        {

        }
    }
}