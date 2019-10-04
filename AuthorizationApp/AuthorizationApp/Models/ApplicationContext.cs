using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationApp.Models
{
    public class ApplicationContext : DbContext 
    {
        public DbSet<Customer> Customers { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            
        }
    }
}
