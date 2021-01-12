using System.Collections.Generic;

namespace Basra.Server.Room
{
    public interface IPending
    {
        int Genre { get; }
        int Id { get; }
        int PlayerCount { get; }
        List<IUser> Users { get; }
    }
}