using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Basra.Server
{
    public class MasterContext : DbContext
    {
        public MasterContext(DbContextOptions<MasterContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Data.PendingRoom> PendingRoom { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //modelBuilder.Entity<User>().HasKey(du => du.Id);
        //    //modelBuilder.Entity<Identity.User>()
        //    //.HasOne(iu => iu.DataUser)
        //    //.WithOne(du => du.IdentityUser)
        //    //.HasForeignKey<User>(du => du.IdentityUserId);

        //}
    }
}
