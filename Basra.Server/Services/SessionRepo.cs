using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Basra.Server.Data;
using Basra.Server.Extensions;
using ConcurrentCollections;

namespace Basra.Server.Services
{
    public interface ISessionRepo
    {
        void DeleteRoom(Room room);
        Room GetPendingRoom(int betChoice, int capacityChoice);

        /// <summary>
        /// if the room is still pending 
        /// </summary>
        void KeepRoom(Room room);

        RoomUser AddRoomUser(string id, string connId, Room room);
        Room MakeRoom(int betChoice, int capacityChoice);

        RoomUser GetRoomUserWithId(string id);

        // bool CheckRoomUserActive(string id);
        // void RemoveRoomUser(string id);
        void RemoveActiveUser(string id);
        bool IsUserActive(string id);
        void AddActiveUser(string id);
        bool DoesRoomUserExist(string id);
    }

    //no test, system funs only
    //may need concurrency test
    public class SessionRepo : ISessionRepo
    {
        //you may have room user that's not active
        //and you may have active user that's not in a room
        //if I made api endpoints before rooms, I will make the connection everytime but it maybe better, I should know
        //the overhead of a connected user
        //so instead of MasterHub, it would be RoomHub
        // private ConcurrentDictionary<string, ActiveUser> ActiveUsers;
        private ConcurrentDictionary<int, Room> Rooms;
        private ConcurrentHashSet<string> ActiveUsers;
        private ConcurrentDictionary<string, RoomUser> RoomUsers;

        private int LastRoomId;
        private int LastRoomUserId;
        private int LastActiveUserId;

        private ConcurrentDictionary<(int, int), ConcurrentBag<Room>> PendingRooms;

        private readonly int[] GenrePosses = { 0, 1, 2, 3 };
        private readonly int[] UserCountPosses = { 2, 3, 4 };

        public SessionRepo()
        {
            Rooms = new ConcurrentDictionary<int, Room>();
            RoomUsers = new ConcurrentDictionary<string, RoomUser>();
            ActiveUsers = new ConcurrentHashSet<string>();

            PendingRooms = new ConcurrentDictionary<(int, int), ConcurrentBag<Room>>();
            for (int i = 0; i < GenrePosses.Length; i++)
            {
                for (int j = 0; j < UserCountPosses.Length; j++)
                {
                    PendingRooms.TryAdd((i, j), new ConcurrentBag<Room>());
                }
            }
        }


        public void DeleteRoom(Room room)
        {
            Rooms.TryRemove(room.Id, out _);
        }


        /// <summary>
        /// if the room is still pending 
        /// </summary>
        public Room MakeRoom(int betChoice, int capacityChoice)
        {
            var room = new Room(betChoice, capacityChoice);

            Rooms.Append(ref LastRoomId, room);
            // PendingRooms[(betChoice, capacityChoice)].Add(room);

            return room;
        }

        public Room GetPendingRoom(int betChoice, int capacityChoice)
        {
            PendingRooms[(betChoice, capacityChoice)].TryTake(out Room room);
            return room;
        }

        public void KeepRoom(Room room)
        {
            PendingRooms[(room.BetChoice, room.CapacityChoice)].Add(room);
        }

        public RoomUser AddRoomUser(string id, string connId, Room room)
        {
            var rUser = new RoomUser { UserId = id, ConnectionId = connId, Room = room };

            room.RoomUsers.Add(rUser);
            RoomUsers.TryAdd(id, rUser);

            return rUser;
        }

        public RoomUser GetRoomUserWithId(string id)
        {
            RoomUsers.TryGetValue(id, out RoomUser roomUser);
            return roomUser;
        }

        public bool DoesRoomUserExist(string id)
        {
            return RoomUsers.ContainsKey(id);
        }
        // public void StartRoomUser(RoomUser roomUser, int turnId, string roomId)
        // {
        //     roomUser.Id = turnId;
        //     roomUser.RoomId = roomId;
        //     
        //     
        // }

        // public bool CheckRoomUserActive(string id)
        // {
        //     return RoomUsers.ContainsKey(id);
        // }
        //
        // public void RemoveRoomUser(string id)
        // {
        //     RoomUsers.TryRemove(id, out _);
        // }

        public bool IsUserActive(string id)
        {
            return ActiveUsers.Contains(id);
        }

        public void RemoveActiveUser(string id)
        {
            ActiveUsers.TryRemove(id);
        }

        public void AddActiveUser(string id)
        {
            ActiveUsers.Add(id);
        }
    }
}