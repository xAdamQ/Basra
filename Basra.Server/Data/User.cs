using System.ComponentModel.DataAnnotations.Schema;

namespace Basra.Server
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Fbid { get; set; }

        public int PlayedGames { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }

        public int Money { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}