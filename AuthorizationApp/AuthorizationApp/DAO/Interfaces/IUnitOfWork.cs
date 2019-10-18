using AuthorizationApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.DAO.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // UserRepository is not done yet
        IRepository<AppUser> Users { get; }
        IRepository<Customer> Customers { get; }

        void Save();
    }
}
