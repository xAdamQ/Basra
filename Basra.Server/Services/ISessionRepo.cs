namespace Basra.Server.Services
{
    public interface ISessionRepo
    {
        void DeleteRoom(Room room);
        Room GetPendingRoom(int genre, int capacity);

        /// <summary>
        /// if the room is still pending 
        /// </summary>
        void KeepRoom(Room room);

        RoomUser AddRoomUser(string id, string connId, Room room);
        Room MakeRoom(int genre, int capacity);
        RoomUser GetRoomUserWithId(string id);
        bool CheckRoomUserActive(string id);
        void RemoveRoomUser(string id);
    }
}