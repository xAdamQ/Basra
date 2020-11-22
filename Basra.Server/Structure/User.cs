namespace Basra.Server.Structure
{
    public class User
    {
        public string Id;
        public Room Room { get; set; }
        public string ConnectionId { get; set; }
        public bool Disconncted { get; set; }

        public void PlayCard(int cardIndex)
        {

        }

        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        public void Ready()
        {
            Room.Ready(Id);
        }
    }
}