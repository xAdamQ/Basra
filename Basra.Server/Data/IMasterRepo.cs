using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Basra.Server
{
    public interface IMasterRepo
    {
        Task<User> CreateUserAsync(string fbid);
        Task<string> GetNameOfUserAsync(string id);
        bool GetUserActiveState(string id);
        Task<User> GetUserByFbidAsync(string fbid);
        Task<User> GetUserByIdAsyc(string id);
        void MarkAllUsersNotActive();
        bool SaveChanges();
        void UpdateFieldsSave<T>(T entity, params Expression<Func<T, object>>[] includeProperties);

        Task<Data.PendingRoom> GetPendingRoomWithSpecs(int genre, int playerCount);
        Task<Data.PendingRoom> CreatePendingRoom(int genre, int playerCount);
    }
}