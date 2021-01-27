using System.ComponentModel.DataAnnotations.Schema;

namespace Basra.Server
{
    public class User
    {
        //how to save -- on singup
        //how to load -- on signin
        //how to access -- 
        //how to update --

        //public ActiveUser ActiveUser;

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