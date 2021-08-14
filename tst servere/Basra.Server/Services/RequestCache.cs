using System;
using System.Threading.Tasks;

namespace Basra.Server.Services
{
    public interface IRequestCache
    {
        void Init(string userId);
        RoomUser RoomUser { get; }
        ActiveUser ActiveUser { get; }
        Task<User> GetUser();
    }

    public class RequestCache : IRequestCache
    {
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;
        public RequestCache(IMasterRepo masterRepo, ISessionRepo sessionRepo)
        {
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
        }

        private string UserId { get; set; }

        private bool Inited;
        public void Init(string userId)
        {
            if (Inited) throw new Exception("the request cache can't be inited twice");
            Inited = true;

            UserId = userId;
        }

        private RoomUser roomUser;
        public RoomUser RoomUser => roomUser ??= _sessionRepo.GetRoomUserWithId(UserId);

        private User user;
        public async Task<User> GetUser() => user ??= await _masterRepo.GetUserByIdAsyc(UserId);

        private ActiveUser activeUser;
        public ActiveUser ActiveUser => activeUser ??= _sessionRepo.GetActiveUser(UserId);
    }
}