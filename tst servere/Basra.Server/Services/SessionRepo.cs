using Basra.Server.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Basra.Server.Services
{
    public interface ISessionRepo
    {
        Room MakeRoom(int betChoice, int capacityChoice);

        /// <summary>
        /// if the room is still pending 
        /// </summary>
        void KeepPendingRoom(Room room);
        Room TakePendingRoom(int betChoice, int capacityChoice);
        //important: we don't remove pending rooms, TakePendingRoom excludes them

        void DeleteRoom(Room room);

        void AddRoomUser(RoomUser roomUser);
        RoomUser GetRoomUserWithId(string id);
        bool DoesRoomUserExist(string id);

        ActiveUser GetActiveUser(string id);
        bool IsUserActive(string id);
        void RemoveActiveUser(string id);
        void AddActiveUser(ActiveUser activeUser);
        void DeleteRoomUser(RoomUser roomUser);
    }


    /// <summary>
    /// collection holder
    /// all it's operations are on collection
    /// </summary>
    public class SessionRepo : ISessionRepo
    {
        private readonly ILogger<SessionRepo> _logger;

        //you may have room user that's not active
        //and you may have active user that's not in a room
        //if I made api endpoints before rooms, I will make the connection everytime but it maybe better, I should know
        //the overhead of a connected user
        //so instead of MasterHub, it would be RoomHub
        // private ConcurrentDictionary<string, ActiveUser> ActiveUsers;
        private ConcurrentDictionary<int, Room> Rooms;
        private ConcurrentDictionary<string, ActiveUser> ActiveUsers;
        private ConcurrentDictionary<string, RoomUser> RoomUsers;

        private int LastRoomId;

        private ConcurrentDictionary<(int, int), ConcurrentBag<Room>> PendingRooms;

        private readonly int[] GenrePosses = {0, 1, 2, 3};
        private readonly int[] UserCountPosses = {2, 3, 4};

        public SessionRepo(ILogger<SessionRepo> logger)
        {
            _logger = logger;
            Rooms = new ConcurrentDictionary<int, Room>();
            RoomUsers = new ConcurrentDictionary<string, RoomUser>();
            ActiveUsers = new ConcurrentDictionary<string, ActiveUser>();

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

        public void DeleteRoomUser(RoomUser roomUser)
        {
            RoomUsers.TryRemove(roomUser.Id, out _);
        }

        /// <summary>
        /// if the room is still pending 
        /// </summary>
        public Room MakeRoom(int betChoice, int capacityChoice)
        {
            var room = new Room(betChoice, capacityChoice);

            Rooms.Append(ref LastRoomId, room);

            return room;
        }

        /// <summary>
        /// takes possible rooms, excludes active amd null rooms
        /// </summary>
        public Room TakePendingRoom(int betChoice, int capacityChoice)
        {
            var bag = PendingRooms[(betChoice, capacityChoice)];

            while (true)
            {
                if (bag.IsEmpty) return null;

                // bag.TryDequeue(out Room room);
                bag.TryTake(out Room room);

                if (room != null && !room.IsFull) return room;
            }
        }

        public void KeepPendingRoom(Room room)
        {
            PendingRooms[(room.BetChoice, room.CapacityChoice)].Add(room);
            // PendingRooms[(room.BetChoice, room.CapacityChoice)].Enqueue(room);
        }

        public void AddRoomUser(RoomUser roomUser)
        {
            RoomUsers.TryAdd(roomUser.Id, roomUser);
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

        public ActiveUser GetActiveUser(string id)
        {
            ActiveUsers.TryGetValue(id, out var activeUser);
            return activeUser;
        }

        public bool IsUserActive(string id)
        {
            return ActiveUsers.ContainsKey(id);
        }

        public void RemoveActiveUser(string id)
        {
            var result = ActiveUsers.TryRemove(id, out _);

            if (!result) _logger.LogWarning($"removing active user was id {id} faild");
        }

        public void AddActiveUser(ActiveUser activeUser)
        {
            var result = ActiveUsers.TryAdd(activeUser.Id, activeUser);

            if (!result) _logger.LogWarning($"adding active user was id {activeUser.Id} faild");
        }
    }
}