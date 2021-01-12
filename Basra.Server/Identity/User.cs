using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Basra.Server.Identity
{
    public class User : IdentityUser
    {
        public string FbId { get; set; }
        //some stuff are runtime
        //so the runtime user is not the same as the stored user

        public Data.User DataUser;
    }
}