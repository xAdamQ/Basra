using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basra.Server.Data
{
    public class GameRepo
    {
        private GameContext _context;

        public GameRepo(GameContext context)
        {
            _context = context;
        }

        public async Task CreateUser(string id)
        {
            await _context.Users.AddAsync(new User { IdentityUserId = id });
        }
    }
}
