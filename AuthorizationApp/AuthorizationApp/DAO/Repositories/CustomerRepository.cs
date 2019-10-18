using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApp.DAO.Interfaces;
using AuthorizationApp.Models;

namespace AuthorizationApp.DAO.Repositories
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly ApplicationContext db;

        public CustomerRepository(ApplicationContext db)
        {
            this.db = db;
        }
        public async void Create(Customer item)
        {
            await db.Customers.AddAsync(item);
        }
    }
}
