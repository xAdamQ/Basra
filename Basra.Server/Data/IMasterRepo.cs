using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Basra.Server.Data;

namespace Basra.Server
{
    public interface IMasterRepo
    {
        Task<User> CreateUserAsync(string fbid);
        Task<string> GetUserNameAsync(string id);
        bool GetUserActiveState(string id);
        Task<User> GetUserByFbidAsync(string fbid);
        Task<User> GetUserByIdAsyc(string id);
        void MarkAllUsersNotActive();
        bool SaveChanges();
        void UpdateFieldsSave<T>(T entity, params Expression<Func<T, object>>[] includeProperties);

        Task<Room> GetRoomWithSpecs(int genre, int playerCount);
        Task<Room> CreateRoom(int genre, int playerCount);

        void SetRoomUserReadyState(string userId);
        Task<bool> GetRoomUserReadyState(string userId);
        Task<bool> GetUserConnectedState(string userId);
        Task<RoomUser> CreateRoomUserAsync(string userId, string connectionId, string roomId);
        Task<string[]> GetRoomUserIds(string roomId);
        Task<string[]> GetRoomUserConnIds(string roomId);
    }
}