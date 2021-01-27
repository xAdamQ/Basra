using System.Threading.Tasks;

namespace Basra.Server
{
    public interface IUser
    {
        string Id { get; set; }
        string Fbid { get; set; }
        int Money { get; set; }
        string Name { get; set; }
        int PlayedGames { get; set; }
        int Draws { get; set; }
        int Wins { get; set; }
    }
}