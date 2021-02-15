using System.Collections.Generic;

namespace Basra.Server
{
    public interface IPendingRoom
    {
        int Genre { get; }
        int Id { get; }
        int PlayerCount { get; }
        List<RoomUser> Users { get; }
    }
}