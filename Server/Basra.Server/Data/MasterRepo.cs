using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Basra.Common;
using Basra.Server.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Basra.Server
{
    public interface IMasterRepo
    {
        Task<User> CreateUserAsync(User user);

        Task<string> GetNameOfUserAsync(string id);

        // bool GetUserActiveState(string id);
        Task<User> GetUserByEIdAsync(string eId, int eIdType);

        Task<User> GetUserByIdAsyc(string id);

        // void MarkAllUsersNotActive();
        Task<bool> SaveChangesAsync();
        // List<DisplayUser> GetRoomDisplayUsersAsync(Room room);

        //
        // // Task<Room> GetPendingRoomWithSpecs(int genre, int playerCount);
        // // Task<Room> CreatePendingRoom(int genre, int playerCount);
        // PendingRoom GetPendingRoomWithSpecs(int genre, int playerCount);
        // PendingRoom MakeRoom(int genre, int userCount);
        // RoomUser GetRoomUserWithId(string id);
        // void DeleteRoom(Room room);
        // RoomUser AddRoomUser(string id, string connId, PendingRoom pRoom);
        // void RemovePendingRoom(PendingRoom pendingRoom);
        //
        // List<DisplayUser> GetRoomDisplayUsersAsync(PendingRoom pendingRoom);
        // void StartRoomUser(RoomUser roomUser, int turnId, string roomId);
        Task<List<User>> GetUsersByIdsAsync(List<string> ids);
        Task<FullUserInfo> GetFullUserInfoAsync(string id);
        Task<List<FullUserInfo>> GetFullUserInfoListAsync(IEnumerable<string> ids);

        Task<List<MinUserInfo>> GetFollowingsAsync(string userId);
        Task<List<MinUserInfo>> GetFollowersAsync(string userId);
        void ToggleFollow(string userId, string targetId);
        bool IsFollowing(string userId, string targetId);
        FriendShip GetFriendship(string userId, string targetId);
        Task CreateExternalId(ExternalId externalId);
    }

    /// <summary>
    /// hide queries
    /// reause queries
    /// </summary>
    public partial class MasterRepo : IMasterRepo
    {
        private readonly MasterContext _context;

        public MasterRepo(MasterContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        #region user

        public async Task CreateExternalId(ExternalId externalId)
        {
            await _context.AddAsync(externalId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.AddAsync(user);
            //await _context.Users.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserByEIdAsync(string eId, int eIdType)
        {
            return await _context.Users.Join(
                _context.ExternalIds.Where(id => id.Type == eIdType && id.MainId == eId),
                u => u.Id, id => id.MainId,
                (u, _) => u).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsyc(string id)
        {
            return await _context.Users.FirstAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<string> ids)
        {
            return await _context.Users.Where(u => ids.Contains(u.Id)).Take(ids.Count)
                .ToListAsync();
        }

        public async Task<string> GetNameOfUserAsync(string id)
        {
            var q = _context.Users.Where(u => u.Id == id).Select(u => u.Name);

            return await _context.Users.Where(u => u.Id == id).Select(u => u.Name).FirstAsync();
        }

        public async Task<FullUserInfo> GetFullUserInfoAsync(string id)
        {
            return await _context.Users.Where(_ => _.Id == id)
                .Select(Mapper.UserToFullUserInfoProjection).FirstAsync();
        }

        public async Task<List<FullUserInfo>> GetFullUserInfoListAsync(IEnumerable<string> ids)
        {
            return await _context.Users.Where(u => ids.Contains(u.Id)).Take(ids.Count())
                .Select(Mapper.UserToFullUserInfoProjection)
                .ToListAsync();
        }

        #endregion

        #region user relation

        public void ToggleFollow(string userId, string targetId)
        {
            if (IsFollower(userId, targetId)) //unfollow
                _context.Remove(new UserRelation {FollowerId = userId, FollowingId = targetId});
            else //follow
                _context.Add(new UserRelation {FollowerId = userId, FollowingId = targetId});
        }

        /// <summary>
        /// Am I follower
        /// </summary>
        public bool IsFollower(string userId, string targetId)
        {
            return _context.UserRelations.Any(r =>
                r.FollowerId == userId && r.FollowingId == targetId);
        }

        /// <summary>
        /// Is HE following me
        /// </summary>
        public bool IsFollowing(string userId, string targetId)
        {
            return _context.UserRelations.Any(r =>
                r.FollowingId == userId && r.FollowerId == targetId);
        }

        public FriendShip GetFriendship(string userId, string targetId)
        {
            var isFollower = IsFollower(userId, targetId);
            var isFollowing = IsFollowing(userId, targetId);

            if (isFollower && isFollowing)
                return FriendShip.Friend;
            if (isFollower)
                return FriendShip.Follower;
            if (isFollowing)
                return FriendShip.Following;

            return FriendShip.None;
        }

        public async Task<List<MinUserInfo>> GetFollowingsAsync(string userId)
        {
            var relationsWhereIFolllow = _context.UserRelations.Where(u => u.FollowerId == userId);

            var myFollowingInfo = relationsWhereIFolllow.Join(_context.Users,
                relation => relation.FollowingId, u => u.Id,
                (_, u) => Mapper.UserToMinUserInfoFunc(u));

            return await myFollowingInfo.ToListAsync();
        }

        public async Task<List<MinUserInfo>> GetFollowersAsync(string userId)
        {
            var relationsWhereIFolllow = _context.UserRelations.Where(u => u.FollowingId == userId);

            var myFollowingInfo = relationsWhereIFolllow.Join(_context.Users,
                relation => relation.FollowerId, u => u.Id,
                (_, u) => Mapper.UserToMinUserInfoFunc(u));

            return await myFollowingInfo.ToListAsync();
        }

        #endregion
    }
}