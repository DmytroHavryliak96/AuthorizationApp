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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
