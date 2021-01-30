using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Data;
using System.Linq.Expressions;

namespace Basra.Server
{
    /// <summary>
    /// hide queries
    /// reause queries
    /// </summary>
    public class MasterRepo : IMasterRepo
    {
        private MasterContext _context;

        public MasterRepo(MasterContext context)
        {
            _context = context;
        }

        //how to use exp
        //undertand db updates
        //try to get sql statement



        public void UpdateFieldsSave<T>(T entity, params System.Linq.Expressions.Expression<Func<T, object>>[] includeProperties)
        {
            var dbEntry = _context.Entry(entity);

            foreach (var includeProperty in includeProperties)
            {
                //includeProperty.Compile().Invoke();
                //dbEntry.Property(includeProperty.).IsModified = true;
            }

            _context.SaveChanges();
        }

        public async Task<User> CreateUserAsync(string fbid)
        {
            var user = new User { Fbid = fbid };
            await _context.AddAsync(user);
            //await _context.Users.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserByFbidAsync(string fbid)
        {
            return await _context.Users.FirstAsync(u => u.Fbid == fbid);
        }
        public async Task<User> GetUserByIdAsyc(string id)
        {
            return await _context.Users.FirstAsync(u => u.Id == id);
        }

        public async Task<string> GetUserNameAsync(string id)
        {
            //var q = _context.Users.Where(u => u.Id == id).Select(u => u.Name);
            //Console.WriteLine("the q iiis:   " + q.ToQueryString());
            return await _context.Users.Where(u => u.Id == id).Select(u => u.Name).FirstAsync();
        }
        public bool GetUserActiveState(string id)
        {
            //_context.E
            return _context.Users.Where(u => u.Id == id).Select(u => u.IsActive).First();
        }

        public void MarkAllUsersNotActive()
        {
            _context.Users.ToList().ForEach(u => u.IsActive = false);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public async Task<bool> GetUserConnectedState(string userId)
        {
            var user = await GetUserByIdAsyc(userId);
            return user.IsActive;
        }

        //////////////////////////////////////////////////////////////////////////room


        public async Task<RoomUser> CreateRoomUserAsync(string userId, string connectionId, string roomId)
        {
            var rUser = new RoomUser { UserId = userId, ConnectionId = connectionId, RoomId = roomId };
            var rUserEntry = await _context.AddAsync(rUser);
            return rUserEntry.Entity;
        }
        public async Task<RoomUser> GetRoomUserByIdAsync(string id)
        {
            return await _context.RoomUsers.FirstAsync(ru => ru.UserId == id);
        }

        public async Task<Room> CreateRoom(int genre, int userCount)
        {
            var room = new Room { Genre = genre, UserCount = userCount };
            var pRoomEntry = await _context.AddAsync(room);
            return pRoomEntry.Entity;
        }
        public async Task<Room> GetRoomWithSpecs(int genre, int userCount)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.Genre == genre && r.UserCount == userCount);
        }

        public void SetRoomUserReadyState(string userId)
        {
            _context.UpdateProperty(new RoomUser { UserId = userId, IsReady = true }, u => u.IsReady);
        }
        public async Task<bool> GetRoomUserReadyState(string userId)
        {
            return await _context.RoomUsers.Where(u => u.UserId == userId).Select(u => u.IsReady).FirstAsync();

            //var rUser = await GetRoomUserByIdAsync(userId);
            //return rUser.IsReady;
        }

        public async Task<string[]> GetRoomUserIds(string roomId)
        {
            return await _context.RoomUsers.Where(u => u.RoomId == roomId).Select(r => r.UserId).ToArrayAsync();
        }
        public async Task<string[]> GetRoomUserConnIds(string roomId)
        {
            return await _context.RoomUsers.Where(u => u.RoomId == roomId).Select(r => r.ConnectionId).ToArrayAsync();
        }

    }

    public static class ContextExtentions
    {
        //public static void UpdateProperty<T>(this DbContext dbContext, T entity, string changedPropName)
        //{
        //    dbContext.Attach(entity);
        //    dbContext.Entry(entity).Property(changedPropName).IsModified = true;
        //}
        //public static void UpdateProperty2<T>(this DbContext dbContext, T entity, Expression<Func<T, object>> includeProperty)
        //{
        //    dbContext.Attach(entity);
        //    dbContext.Entry(entity).Property(nameof(includeProperty.Body)).IsModified = true;
        //}
        //public static void UpdateProperty3<T>(this DbContext dbContext, T entity, object prop)
        //{
        //    dbContext.Attach(entity);
        //    dbContext.Entry(entity).Property(nameof(prop)).IsModified = true;
        //}

        public static void UpdateProperty<T>(this DbContext dbContext, T entity, Expression<Func<T, object>> includeProperty)
        {
            dbContext.Attach(entity);
            dbContext.Entry(entity).Property(GetMemeberName(includeProperty)).IsModified = true;
        }
        public static void UpdateProperties<T>(this DbContext dbContext, T entity, params Expression<Func<T, object>>[] includeProperties)
        {
            dbContext.Attach(entity);
            foreach (var includeProperty in includeProperties)
            {
                dbContext.Entry(entity).Property(GetMemeberName(includeProperty)).IsModified = true;
            }
        }

        //public static void UpdateFieldsSave<T>(this DbContext context, T entity, Expression<Func<T, object>> includeProperty)
        //{
        //    var dbEntry = context.Entry(entity);

        //    //dbEntry.Property(includeProperty.).IsModified = true;

        //    context.SaveChanges();
        //}
        //public static void UpdateFieldsSave<T>(this DbContext context, T entity, params Expression<Func<T, object>>[] includeProperties)
        //{
        //var dbEntry = context.Entry(entity);

        //foreach (var includeProperty in includeProperties)
        //{
        //dbEntry.Property(nameof(includeProperty)).IsModified = true;
        //}

        //context.SaveChanges();
        //}

        public static string GetMemeberName<T>(Expression<Func<T, object>> expression)
        {
            var unaryExpo = expression.Body as UnaryExpression;
            var operand = unaryExpo.Operand;
            var memExp = operand as MemberExpression;
            return memExp.Member.Name;
        }
    }
}
