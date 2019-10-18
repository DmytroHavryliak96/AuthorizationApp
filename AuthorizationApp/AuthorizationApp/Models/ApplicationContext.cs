using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationApp.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<IdentityUserLogin<string>> IdentityUserLogins {get; set;}

        /*public DbSet<IdentityUserClaim<string>> IdentityUserClaim { get; set; }

        public DbSet<IdentityUserRole<string>> IdentitytUserRole { get; set; }

        public DbSet<IdentityRole> Roles { get; set; }*/

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<AppUser>()
                .HasOne(ap => ap.Customer)
                .WithOne(c => c.Identity)
                .HasForeignKey<Customer>(c => c.IdentityId);

            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins")
            .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });



            /*modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles")
            .HasKey(r => new { r.UserId, r.RoleId });

            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims").HasKey(p => new { p.Id });*/
        }
    }
    
}
