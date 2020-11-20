using System.Collections.Generic;

namespace Basra.Server.Structure
{
    public struct Room
    {
        public int Genre { get; set; }
        public List<string> Players { get; set; } //maybe removed if the groups is utilized
        public int PlayerCount { get; set; }
        public int Id { get; set; }
        public static int LastId { get; set; }
    }
}