using Basra.Server.Data;

namespace Basra.Server
{
    public interface IAppManger
    {
        public User GetPersonalData();
        public DisplayUser GetUserData(string id); //including friends
        public void ReceiveMoneyAid(); //the timer is started manually

        /// <summary>
        /// for users that reconnected to the room and want it's current state
        /// </summary>
        public RoomData GetRoomData();
    }

    public class AppManager : IAppManger
    {
        public User GetPersonalData()
        {
            throw new System.NotImplementedException();
        }
        public DisplayUser GetUserData(string id)
        {
            throw new System.NotImplementedException();
        }
        public void ReceiveMoneyAid()
        {
            throw new System.NotImplementedException();
        }
        public RoomData GetRoomData()
        {
            throw new System.NotImplementedException();
        }
    }
}