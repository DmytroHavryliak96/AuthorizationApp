using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationApp.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<IdentityUserLogin<string>> IdentityUserLogins {get; set;}

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

        }
    }
    
}
