using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Basra.Models.Client;
using Basra.Server.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Basra.Server
{
    public interface IMasterRepo
    {
        Task<User> CreateUserAsync(string fbid);

        Task<string> GetNameOfUserAsync(string id);

        // bool GetUserActiveState(string id);
        Task<User> GetUserByFbidAsync(string fbid);

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
        Task<List<User>> GetUsersByIds(List<string> ids);
        Task<FullUserInfo> GetFullUserInfoAsync(string id);
        Task<List<FullUserInfo>> GetFullUserInfoListAsync(List<string> ids);
    }

    /// <summary>
    /// hide queries
    /// reause queries
    /// </summary>
    public class MasterRepo : IMasterRepo
    {
        private readonly MasterContext _context;

        public MasterRepo(MasterContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return _context.SaveChanges() >= 0;
        }

        #region user

        public async Task<User> CreateUserAsync(string fbid)
        {
            var user = new User {Fbid = fbid};
            await _context.AddAsync(user);
            //await _context.Users.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserByFbidAsync(string fbid)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Fbid == fbid);
        }

        public async Task<User> GetUserByIdAsyc(string id)
        {
            return await _context.Users.FirstAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetUsersByIds(List<string> ids)
        {
            return await _context.Users.Where(u => ids.Contains(u.Id)).Take(ids.Count).ToListAsync();
        }

        public async Task<string> GetNameOfUserAsync(string id)
        {
            var q = _context.Users.Where(u => u.Id == id).Select(u => u.Name);

            return await _context.Users.Where(u => u.Id == id).Select(u => u.Name).FirstAsync();
        }

        // public bool GetUserActiveState(string id)
        // {
        //     return _context.Users.Where(u => u.Id == id).Select(u => u.IsActive).First();
        // }

        // public void MarkAllUsersNotActive()
        // {
        //     _context.Users.ToList().ForEach(u => u.IsActive = false);
        // }

        //todo should test query
        // public List<DisplayUser> GetRoomDisplayUsersAsync(Room room)
        // {
        //     // return _context.Entry(pendingRoom)
        //     //     .Reference(pr => pr.Room)
        //     //     .Query()
        //     //     .SelectMany(c => c.RoomUsers)
        //     //     .Select(ru => ru.User)
        //     //     .Select(DisplayUser.Projection)
        //     //     .ToList();
        //
        //     var userIds = room.RoomUsers.Select(ru => ru.UserId);
        //     return _context.Users.Where(u => userIds.Contains(u.Id)).Select(DisplayUser.Projection).ToList();
        // }

        // public async Task<DisplayUser> GetDisplayUserAsync(string id)
        // {
        //     return await _context.Users.Where(_ => _.Id == id).Select(DisplayUser.Projection).FirstAsync();
        // }

        public async Task<FullUserInfo> GetFullUserInfoAsync(string id)
        {
            return await _context.Users.Where(_ => _.Id == id).Select(Mapper.UserToFullUserInfoProjection).FirstAsync();
        }
        public async Task<List<FullUserInfo>> GetFullUserInfoListAsync(List<string> ids)
        {
            return await _context.Users.Where(u => ids.Contains(u.Id)).Take(ids.Count)
                .Select(Mapper.UserToFullUserInfoProjection)
                .ToListAsync();
        }

        #endregion
    }
}