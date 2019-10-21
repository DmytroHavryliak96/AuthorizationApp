using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApp.DAO.Interfaces;
using AuthorizationApp.DAO.Repositories;
using AuthorizationApp.Models;

namespace AuthorizationApp.DAO
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext db;

        private CustomerRepository customerRepository;

        
        public EFUnitOfWork(ApplicationContext context)
        {
            db = context;
        } 

        public IRepository<AppUser> Users => throw new NotImplementedException();

        public IRepository<Customer> Customers { 
            get 
            {
                if (customerRepository == null)
                    customerRepository = new CustomerRepository(db);

                return customerRepository;
            
            } 
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;
    }
}
