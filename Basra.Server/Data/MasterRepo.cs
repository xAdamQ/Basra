using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<string> GetNameOfUserAsync(string id)
        {
            var q = _context.Users.Where(u => u.Id == id).Select(u => u.Name);
            Console.WriteLine("the q iiis:   " + q.ToQueryString());

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

        public async Task<Data.PendingRoom> GetPendingRoomWithSpecs(int genre, int playerCount)
        {
            return await _context.PendingRoom.FirstOrDefaultAsync(r => r.Genre == genre && r.PlayerCount == playerCount);
        }

        public async Task<Data.PendingRoom> CreatePendingRoom(int genre, int playerCount)
        {
            var pRoom = new Data.PendingRoom { Genre = genre, PlayerCount = playerCount };
            await _context.AddAsync(pRoom);
            return pRoom;
        }
    }
}
